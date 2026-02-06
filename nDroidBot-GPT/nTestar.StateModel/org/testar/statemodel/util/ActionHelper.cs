using System.Collections.Generic;
using System.Linq;
using org.testar.monkey.alayer;

namespace org.testar.statemodel.util
{
    public static class ActionHelper
    {
        public static ISet<string> GetAbstractIds(ISet<AbstractAction> actions)
        {
            HashSet<string> actionIds = new HashSet<string>();
            foreach (AbstractAction action in actions)
            {
                actionIds.Add(action.GetActionId());
            }

            return actionIds;
        }

        public static ISet<AbstractAction> ConvertActionsToAbstractActions(ISet<Action> actions)
        {
            HashSet<AbstractAction> abstractActions = new HashSet<AbstractAction>();
            if (actions == null || actions.Count == 0)
            {
                return abstractActions;
            }

            var actionMap = actions.GroupBy(a => a.get(Tags.AbstractID));
            foreach (var group in actionMap)
            {
                AbstractAction abstractAction = new AbstractAction(group.Key);
                foreach (Action action in group)
                {
                    abstractAction.AddConcreteActionId(action.get(Tags.ConcreteID));
                }
                abstractActions.Add(abstractAction);
            }

            return abstractActions;
        }
    }
}
