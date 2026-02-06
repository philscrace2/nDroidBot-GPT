using System.Collections.Generic;
using org.testar.monkey.alayer;
using Action = org.testar.monkey.alayer.Action;

namespace org.testar.statemodel
{
    public class DummyModelManager : StateModelManager
    {
        public void NotifyNewStateReached(State newState, HashSet<Action> actions)
        {
        }

        public void NotifyActionExecution(Action action)
        {
        }

        public void NotifyTestingEnded()
        {
        }

        public Action GetAbstractActionToExecute(HashSet<Action> actions)
        {
            return null;
        }

        public void NotifyTestSequencedStarted()
        {
        }

        public void NotifyTestSequenceStopped()
        {
        }

        public void NotifyTestSequenceInterruptedByUser()
        {
        }

        public void NotifyTestSequenceInterruptedBySystem(string message)
        {
        }

        public string GetModelIdentifier()
        {
            return string.Empty;
        }

        public string QueryStateModel(string query, params object[] parameters)
        {
            return string.Empty;
        }
    }
}
