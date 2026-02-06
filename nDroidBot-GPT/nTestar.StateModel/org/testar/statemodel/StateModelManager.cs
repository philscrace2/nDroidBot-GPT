using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using org.testar.monkey.alayer;

namespace org.testar.statemodel
{
    public interface StateModelManager
    {
        void NotifyNewStateReached(State newState, HashSet<Action> actions);

        void NotifyActionExecution(Action action);

        void NotifyTestingEnded();

        Action GetAbstractActionToExecute(HashSet<Action> actions);

        void NotifyTestSequencedStarted();

        void NotifyTestSequenceStopped();

        void NotifyTestSequenceInterruptedByUser();

        void NotifyTestSequenceInterruptedBySystem(string message);

        string GetModelIdentifier();

        string QueryStateModel(string query, params object[] parameters);
    }
}
