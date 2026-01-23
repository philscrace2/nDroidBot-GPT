using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.nTestar.Statemodel
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

        void NotifyTestSequenceInterruptedBySystem(String message);
    }
}
