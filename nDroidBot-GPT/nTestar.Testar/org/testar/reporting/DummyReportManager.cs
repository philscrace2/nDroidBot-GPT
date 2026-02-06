using System.Collections.Generic;
using org.testar.monkey.alayer;
using State = org.testar.monkey.alayer.State;
using Action = org.testar.monkey.alayer.Action;

namespace org.testar.reporting
{
    public class DummyReportManager : Reporting
    {
        public void addState(State state)
        {
        }

        public void addActions(ISet<Action> actions)
        {
        }

        public void addActionsAndUnvisitedActions(ISet<Action> actions, ISet<string> concreteIdsOfUnvisitedActions)
        {
        }

        public void addSelectedAction(State state, Action action)
        {
        }

        public void addTestVerdict(Verdict verdict)
        {
        }

        public void finishReport()
        {
        }
    }
}
