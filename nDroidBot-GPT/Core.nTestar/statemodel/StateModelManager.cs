using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.nTestar.statemodel
{
    public interface StateModelManager
    {
        void notifyNewStateReached(State newState, Set<Action> actions);

        void notifyActionExecution(Action action);

        void notifyTestingEnded();

        Action getAbstractActionToExecute(Set<Action> actions);

        void notifyTestSequencedStarted();

        void notifyTestSequenceStopped();

        void notifyTestSequenceInterruptedByUser();

        void notifyTestSequenceInterruptedBySystem(String message);
    }
}
