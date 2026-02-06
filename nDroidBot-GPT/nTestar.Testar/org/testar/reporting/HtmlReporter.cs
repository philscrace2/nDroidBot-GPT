using System;
using System.Collections.Generic;
using System.Net;
using org.testar.monkey.alayer;
using State = org.testar.monkey.alayer.State;
using Action = org.testar.monkey.alayer.Action;
using org.testar.monkey.alayer.webdriver.enums;
using org.testar.monkey;

namespace org.testar.reporting
{
    public class HtmlReporter : Reporting
    {
        private readonly HtmlFormatUtil htmlReportUtil;
        private int innerLoopCounter;

        private readonly string openStateBlockContainer = "<div class='stateBlock' style='display:flex;flex-direction:column'>";
        private readonly string openDerivedBlockContainer = "<div class='derivedBlock' style='display:flex;flex-direction:column'>";
        private readonly string openSelectedBlockContainer = "<div class='selectedBlock' style='display:flex;flex-direction:column'>";
        private readonly string openVerdictBlockContainer = "<div class='verdictBlock' style='display:flex;flex-direction:column'>";

        private readonly string openBackgroundContainer = "<div class='background'>";
        private readonly string openCollapsibleContainer = "<div class='collapsibleContent'>";
        private readonly string closeContainer = "</div>";

        public HtmlReporter(string fileName, bool replay)
        {
            htmlReportUtil = new HtmlFormatUtil(fileName);

            string headerTitle = "TESTAR execution sequence report";
            htmlReportUtil.addHeader(headerTitle, HtmlHelper.getHtmlScript(), HtmlHelper.getHtmlStyle());

            if (replay)
            {
                addReplayHeading();
            }
            else
            {
                addGenerateHeading();
            }
        }

        private void addReplayHeading()
        {
            htmlReportUtil.addHeading(1, "TESTAR replay sequence report for file " + ConfigTags.PathToReplaySequence);
        }

        private void addGenerateHeading()
        {
            htmlReportUtil.addHeading(1, "TESTAR execution sequence report for sequence " + OutputStructure.sequenceInnerLoopCount);
            htmlReportUtil.addContent("<button id='reverseButton' onclick='reverse()'>Reverse order</button>");
            htmlReportUtil.addContent("<div id='main' style='display:flex;flex-direction:column'>");
        }

        public void addState(State state)
        {
            string imagePath = prepareScreenshotImagePath(state.get(Tags.ScreenshotPath, "NoScreenshotPathAvailable"));
            string concreteID = state.get(Tags.ConcreteID, "NoConcreteIdAvailable");
            string abstractID = state.get(Tags.AbstractID, "NoAbstractIdAvailable");

            htmlReportUtil.addContent(openStateBlockContainer);
            htmlReportUtil.addContent(openBackgroundContainer);

            htmlReportUtil.addHeading(2, "State " + innerLoopCounter);

            string stateIDs = "AbstractID=" + abstractID + " || " + "ConcreteID=" + concreteID;
            htmlReportUtil.addHeading(4, stateIDs);

            long stateTimeStamp = state.get(Tags.TimeStamp, 0L);
            DateTimeOffset epochTimeStamp = DateTimeOffset.FromUnixTimeMilliseconds(stateTimeStamp);
            string formattedTimestamp = epochTimeStamp.ToString("O");
            long epochMillis = epochTimeStamp.ToUnixTimeMilliseconds();
            htmlReportUtil.addHeading(4, "TimeStamp: " + formattedTimestamp + " || Epoch: " + epochMillis);

            long stateRenderTimeValue = state.get(Tags.StateRenderTime, -1L);
            if (stateRenderTimeValue >= 0)
            {
                string stateRenderTime = "State Render Time: " + stateRenderTimeValue + " ms";
                htmlReportUtil.addHeading(4, stateRenderTime);
            }

            if (!string.IsNullOrEmpty(state.get(WdTags.WebHref, string.Empty)))
            {
                string stateURL = state.get(WdTags.WebHref, string.Empty);
                string htmlStateURL = "<a href='" + stateURL + "' target='_blank'>" + stateURL + "</a>";
                htmlReportUtil.addContent(htmlStateURL);
            }

            string altText = "screenshot: state=" + innerLoopCounter + ", ConcreteID=" + concreteID + ", AbstractID=" + abstractID;
            htmlReportUtil.addParagraph("<img src=\"" + imagePath + "\" alt=\"" + altText + "\">");

            htmlReportUtil.addContent(closeContainer);
            htmlReportUtil.addContent(closeContainer);

            innerLoopCounter++;
            htmlReportUtil.writeToFile();
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
            var joiner = new List<string>();

            string escaped = WebUtility.HtmlEncode(action.get(Tags.Desc, "NoActionDescriptionAvailable"));
            joiner.Add("<b>" + escaped + "</b>");
            joiner.Add(WebUtility.HtmlEncode(action.ToString()));
            joiner.Add("ConcreteID=" + action.get(Tags.ConcreteID, "NoConcreteIdAvailable"));
            joiner.Add("AbstractID=" + action.get(Tags.AbstractID, "NoAbstractIdAvailable"));

            return string.Join(" || ", joiner);
        }

        public void addActions(ISet<Action> actions)
        {
            htmlReportUtil.addContent(openDerivedBlockContainer);
            htmlReportUtil.addButton("collapsible", "Click to view the set of derived actions:");
            htmlReportUtil.addContent(openCollapsibleContainer);

            var actionStrings = new List<string>();
            foreach (Action action in actions)
            {
                actionStrings.Add(getActionString(action));
            }

            htmlReportUtil.addList(false, actionStrings);

            htmlReportUtil.addContent(closeContainer);
            htmlReportUtil.addContent(closeContainer);

            htmlReportUtil.writeToFile();
        }

        public void addActionsAndUnvisitedActions(ISet<Action> actions, ISet<string> concreteIdsOfUnvisitedActions)
        {
            htmlReportUtil.addContent(openDerivedBlockContainer);

            var actionStrings = new List<string>();
            if (actions.Count == concreteIdsOfUnvisitedActions.Count)
            {
                htmlReportUtil.addHeading(4, "Set of actions (all unvisited - a new state):");
                foreach (Action action in actions)
                {
                    actionStrings.Add(getActionString(action));
                }
            }
            else if (concreteIdsOfUnvisitedActions.Count == 0)
            {
                htmlReportUtil.addHeading(4, "All actions have been visited, set of available actions:");
                foreach (Action action in actions)
                {
                    actionStrings.Add(getActionString(action));
                }
            }
            else
            {
                htmlReportUtil.addHeading(4, concreteIdsOfUnvisitedActions.Count + " out of " + actions.Count + " actions have not been visited yet:");
                foreach (Action action in actions)
                {
                    if (concreteIdsOfUnvisitedActions.Contains(action.get(Tags.ConcreteID, "NoConcreteIdAvailable")))
                    {
                        actionStrings.Add(getActionString(action));
                    }
                }
            }
            htmlReportUtil.addList(false, actionStrings);
            htmlReportUtil.addContent(closeContainer);

            htmlReportUtil.writeToFile();
        }

        public void addSelectedAction(State state, Action action)
        {
            string screenshotDir = prepareScreenshotImagePath(OutputStructure.screenshotsOutputDir ?? string.Empty);
            string stateConcreteID = state.get(Tags.ConcreteID, "NoConcreteIdAvailable");
            string actionAbstractID = action.get(Tags.AbstractID, "NoAbstractIdAvailable");
            string actionConcreteID = action.get(Tags.ConcreteID, "NoConcreteIdAvailable");

            string actionPath = screenshotDir + "/"
                                + OutputStructure.startInnerLoopDateString + "_" + OutputStructure.executedSUTname
                                + "_sequence_" + OutputStructure.sequenceInnerLoopCount
                                + "/" + stateConcreteID
                                + "_" + actionConcreteID + ".png";

            htmlReportUtil.addContent(openSelectedBlockContainer);
            htmlReportUtil.addContent(openBackgroundContainer);

            htmlReportUtil.addHeading(2, "Selected Action " + innerLoopCounter + " leading to State " + innerLoopCounter);

            string actionIDs = "AbstractID=" + actionAbstractID + " || " + "ConcreteID=" + actionConcreteID;
            string escaped = WebUtility.HtmlEncode(action.get(Tags.Desc, "NoActionDescriptionAvailable"));
            actionIDs += " || " + escaped;
            htmlReportUtil.addHeading(4, actionIDs);

            if (actionPath.Contains("./output", StringComparison.Ordinal))
            {
                actionPath = actionPath.Replace("./output", "..", StringComparison.Ordinal);
            }

            actionPath = actionPath.Replace("\\", "/", StringComparison.Ordinal);

            string altText = "screenshot: action, ConcreteID=" + actionConcreteID;

            htmlReportUtil.addParagraph("<img src=\"" + actionPath + "\" alt=\"" + altText + "\">");

            htmlReportUtil.addContent(closeContainer);
            htmlReportUtil.addContent(closeContainer);

            htmlReportUtil.writeToFile();
        }

        public void addTestVerdict(Verdict verdict)
        {
            string verdictInfo = WebUtility.HtmlEncode(verdict.info());
            if (verdict.severity() > Verdict.OK.severity())
            {
                verdictInfo = verdictInfo.Replace(Verdict.OK.info(), string.Empty, StringComparison.Ordinal).Replace("\n", string.Empty, StringComparison.Ordinal);
            }

            htmlReportUtil.addContent(openVerdictBlockContainer);
            htmlReportUtil.addHeading(2, "Test verdict for this sequence: " + verdictInfo);
            htmlReportUtil.addHeading(4, "Severity: " + verdict.severity());
            htmlReportUtil.addContent("<h4 id='visualizer-rect' style='display: none;'>Visualizer: " + verdict.visualizer().getShapes() + "</h4>");
            htmlReportUtil.addContent(closeContainer);

            htmlReportUtil.appendToFileName("_" + verdict.verdictSeverityTitle());
            htmlReportUtil.writeToFile();
        }

        public void finishReport()
        {
            htmlReportUtil.addContent("</div>");
            htmlReportUtil.addFooter();

            htmlReportUtil.writeToFile();
        }
    }
}
