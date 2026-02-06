using System.Collections.Generic;
using org.testar.monkey.alayer;

namespace org.testar.reporting
{
    public interface Reporting
    {
        void addState(State state);
        void addActions(ISet<Action> actions);
        void addActionsAndUnvisitedActions(ISet<Action> actions, ISet<string> concreteIdsOfUnvisitedActions);
        void addSelectedAction(State state, Action action);
        void addTestVerdict(Verdict verdict);
        void finishReport();
    }
}
