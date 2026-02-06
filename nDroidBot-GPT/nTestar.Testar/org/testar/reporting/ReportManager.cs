using System.Collections.Generic;
using org.testar.monkey.alayer;
using org.testar.settings;

namespace org.testar.reporting
{
    public class ReportManager : Reporting
    {
        private readonly List<Reporting> reporters = new List<Reporting>();
        private readonly bool reportingEnabled;
        private bool firstStateAdded;
        private bool firstActionsAdded;
        private readonly string fileName;

        public ReportManager(bool replay, Settings settings)
        {
            fileName = $"{OutputStructure.htmlOutputDir}{System.IO.Path.DirectorySeparatorChar}{OutputStructure.startInnerLoopDateString}_{OutputStructure.executedSUTname}_sequence_{OutputStructure.sequenceInnerLoopCount}";

            bool html = settings.Get(org.testar.monkey.ConfigTags.ReportInHTML, false);
            bool plainText = settings.Get(org.testar.monkey.ConfigTags.ReportInPlainText, false);

            reportingEnabled = html || plainText;
            if (!reportingEnabled)
            {
                return;
            }

            if (html)
            {
                reporters.Add(new HtmlReporter(fileName, replay));
            }

            if (plainText)
            {
                reporters.Add(new PlainTextReporter(fileName, replay));
            }
        }

        public string getReportFileName()
        {
            return fileName;
        }

        public void addState(State state)
        {
            if (!reportingEnabled)
            {
                return;
            }

            if (firstStateAdded)
            {
                if (firstActionsAdded || state.get(Tags.OracleVerdict, Verdict.OK).severity() > 0.0)
                {
                    foreach (var reporter in reporters)
                    {
                        reporter.addState(state);
                    }
                }
            }
            else
            {
                firstStateAdded = true;
                foreach (var reporter in reporters)
                {
                    reporter.addState(state);
                }
            }
        }

        public void addActions(ISet<Action> actions)
        {
            if (!reportingEnabled)
            {
                return;
            }

            firstActionsAdded = true;
            foreach (var reporter in reporters)
            {
                reporter.addActions(actions);
            }
        }

        public void addActionsAndUnvisitedActions(ISet<Action> actions, ISet<string> concreteIdsOfUnvisitedActions)
        {
            if (!reportingEnabled)
            {
                return;
            }

            firstActionsAdded = true;
            foreach (var reporter in reporters)
            {
                reporter.addActionsAndUnvisitedActions(actions, concreteIdsOfUnvisitedActions);
            }
        }

        public void addSelectedAction(State state, Action action)
        {
            if (!reportingEnabled)
            {
                return;
            }

            foreach (var reporter in reporters)
            {
                reporter.addSelectedAction(state, action);
            }
        }

        public void addTestVerdict(Verdict verdict)
        {
            if (!reportingEnabled)
            {
                return;
            }

            foreach (var reporter in reporters)
            {
                reporter.addTestVerdict(verdict);
            }
        }

        public void finishReport()
        {
            if (!reportingEnabled)
            {
                return;
            }

            foreach (var reporter in reporters)
            {
                reporter.finishReport();
            }
        }
    }
}
