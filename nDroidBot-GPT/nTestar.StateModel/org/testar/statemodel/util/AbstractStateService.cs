using System.Collections.Generic;
using org.testar.monkey.alayer;
using Action = org.testar.monkey.alayer.Action;

namespace org.testar.statemodel.util
{
    public static class AbstractStateService
    {
        public static void UpdateAbstractStateActions(AbstractState abstractState, ISet<Action> actions)
        {
            foreach (AbstractAction action in ActionHelper.ConvertActionsToAbstractActions(actions))
            {
                abstractState.AddNewAction(action);
            }
        }
    }
}
