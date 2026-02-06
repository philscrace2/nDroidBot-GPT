using System.Collections.Generic;
using org.testar.monkey.alayer;
using Action = org.testar.monkey.alayer.Action;
using org.testar.statemodel.util;

namespace org.testar.statemodel
{
    internal static class AbstractStateFactory
    {
        public static AbstractState CreateAbstractState(State newState, ISet<Action> actions)
        {
            string abstractStateId = newState.get(Tags.AbstractID);
            return new AbstractState(abstractStateId, ActionHelper.ConvertActionsToAbstractActions(actions));
        }
    }
}
