using System;
using System.Collections.Generic;
using System.Net;
using org.testar.monkey.alayer;
using State = org.testar.monkey.alayer.State;
using Action = org.testar.monkey.alayer.Action;
using org.testar.monkey;

namespace org.testar.reporting
{
    public class PlainTextReporter : Reporting
    {
        private readonly PlainTextFormatUtil plainTextReportUtil;
        private int innerLoopCounter;

        public PlainTextReporter(string fileName, bool replay)
        {
            plainTextReportUtil = new PlainTextFormatUtil(fileName);

            startReport();
            if (replay)
            {
                addReplayHeading();
            }
            else
            {
                addGenerateHeading();
            }
        }

        private void startReport()
        {
            plainTextReportUtil.addHeading(1, "TESTAR execution sequence report");
        }

        private void addReplayHeading()
        {
            plainTextReportUtil.addHeading(2, "TESTAR replay sequence report for file " + ConfigTags.PathToReplaySequence);
        }

        private void addGenerateHeading()
        {
            plainTextReportUtil.addHeading(2, "TESTAR execution sequence report for sequence " + OutputStructure.sequenceInnerLoopCount);
        }

        public void addState(State state)
        {
            string imagePath = prepareScreenshotImagePath(state.get(Tags.ScreenshotPath, "NoScreenshotPathAvailable"));
            string concreteID = state.get(Tags.ConcreteID, "NoConcreteIdAvailable");
            string abstractID = state.get(Tags.AbstractID, "NoAbstractIdAvailable");

            plainTextReportUtil.addHeading(3, "State " + innerLoopCounter);
            plainTextReportUtil.addHeading(5, "ConcreteID=" + concreteID);
            plainTextReportUtil.addHeading(5, "AbstractID=" + abstractID);

            string altText = "screenshot: state=" + innerLoopCounter + ", ConcreteID=" + concreteID + ", AbstractID=" + abstractID;
            plainTextReportUtil.addParagraph("Image: " + imagePath + "\n" + altText);

            innerLoopCounter++;
            plainTextReportUtil.writeToFile();
        }

        private string prepareScreenshotImagePath(string path)
        {
            if (path.Contains("./output", StringComparison.Ordinal))
            {
                int indexStart = path.IndexOf("./output", StringComparison.Ordinal);
                int indexScrn = path.IndexOf("scrshots", StringComparison.Ordinal);
                string replaceString = path.Substring(indexStart, indexScrn - indexStart);
                path = path.Replace(replaceString, "../", StringComparison.Ordinal);
            }
            return path.Replace("\\", "/", StringComparison.Ordinal);
        }

        private string getActionString(Action action)
        {
            var parts = new List<string>();

            string escaped = WebUtility.HtmlEncode(action.get(Tags.Desc, "NoActionDescriptionAvailable"));
            parts.Add(escaped);
            parts.Add(WebUtility.HtmlEncode(action.ToString()));
            parts.Add("ConcreteID=" + action.get(Tags.ConcreteID, "NoConcreteIdAvailable"));
            parts.Add("AbstractID=" + action.get(Tags.AbstractID, "NoAbstractIdAvailable"));

            return string.Join(" || ", parts);
        }

        public void addActions(ISet<Action> actions)
        {
            plainTextReportUtil.addHeading(4, "Set of actions:");

            var actionStrings = new List<string>();
            foreach (Action action in actions)
            {
                actionStrings.Add(getActionString(action));
            }

            plainTextReportUtil.addList(false, actionStrings);

            plainTextReportUtil.writeToFile();
        }

        public void addActionsAndUnvisitedActions(ISet<Action> actions, ISet<string> concreteIdsOfUnvisitedActions)
        {
            var actionStrings = new List<string>();
            if (actions.Count == concreteIdsOfUnvisitedActions.Count)
            {
                plainTextReportUtil.addHeading(5, "Set of actions (all unvisited - a new state):");
                foreach (Action action in actions)
                {
                    actionStrings.Add(getActionString(action));
                }
            }
            else if (concreteIdsOfUnvisitedActions.Count == 0)
            {
                plainTextReportUtil.addHeading(5, "All actions have been visited, set of available actions:");
                foreach (Action action in actions)
                {
                    actionStrings.Add(getActionString(action));
                }
            }
            else
            {
                plainTextReportUtil.addHeading(5, concreteIdsOfUnvisitedActions.Count + " out of " + actions.Count + " actions have not been visited yet:");
                foreach (Action action in actions)
                {
                    if (concreteIdsOfUnvisitedActions.Contains(action.get(Tags.ConcreteID, "NoConcreteIdAvailable")))
                    {
                        actionStrings.Add(getActionString(action));
                    }
                }
            }
            plainTextReportUtil.addList(false, actionStrings);

            plainTextReportUtil.writeToFile();
        }

        public void addSelectedAction(State state, Action action)
        {
            string screenshotDir = prepareScreenshotImagePath(OutputStructure.screenshotsOutputDir ?? string.Empty);
            string stateConcreteID = state.get(Tags.ConcreteID, "NoConcreteIdAvailable");
            string actionConcreteID = action.get(Tags.ConcreteID, "NoConcreteIdAvailable");

            string actionPath = screenshotDir + "/"
                                + OutputStructure.startInnerLoopDateString + "_" + OutputStructure.executedSUTname
                                + "_sequence_" + OutputStructure.sequenceInnerLoopCount
                                + "/" + stateConcreteID
                                + "_" + actionConcreteID + ".png";

            plainTextReportUtil.addHeading(3, "Selected Action " + innerLoopCounter + " leading to State " + innerLoopCounter);

            string stateString = "ConcreteID=" + actionConcreteID;
            string escaped = WebUtility.HtmlEncode(action.get(Tags.Desc, "NoActionDescriptionAvailable"));
            stateString += " || " + escaped;
            plainTextReportUtil.addHeading(5, stateString);

            if (actionPath.Contains("./output", StringComparison.Ordinal))
            {
                actionPath = actionPath.Replace("./output", "..", StringComparison.Ordinal);
            }

            actionPath = actionPath.Replace("\\", "/", StringComparison.Ordinal);

            string altText = "screenshot: action, ConcreteID=" + actionConcreteID;

            plainTextReportUtil.addParagraph("Image: " + actionPath + "\n" + altText);

            plainTextReportUtil.writeToFile();
        }

        public void addTestVerdict(Verdict verdict)
        {
            string verdictInfo = verdict.info();
            if (verdict.severity() > Verdict.OK.severity())
            {
                verdictInfo = verdictInfo.Replace(Verdict.OK.info(), string.Empty, StringComparison.Ordinal);
            }

            plainTextReportUtil.addHorizontalLine();
            plainTextReportUtil.addHeading(3, "Test verdict for this sequence: " + verdictInfo);
            plainTextReportUtil.addHeading(5, "Severity: " + verdict.severity());

            plainTextReportUtil.appendToFileName("_" + verdict.verdictSeverityTitle());
            plainTextReportUtil.writeToFile();
        }

        public void finishReport()
        {
            plainTextReportUtil.writeToFile();
        }
    }
}
