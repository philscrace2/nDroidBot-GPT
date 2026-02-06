using System;
using System.Collections.Generic;
using org.testar.statemodel.exceptions;

namespace org.testar.statemodel.actionselector
{
    public class RandomActionSelector : ActionSelector
    {
        public void NotifyNewSequence()
        {
        }

        public AbstractAction SelectAction(AbstractState currentState, AbstractStateModel abstractStateModel)
        {
            long graphTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            Random rnd = new Random((int)(graphTime & 0x7FFFFFFF));
            IReadOnlyCollection<string> actionIds = currentState.GetActionIds();
            var actionList = new List<string>(actionIds);
            string actionId = actionList[rnd.Next(actionList.Count)];
            return currentState.GetAction(actionId);
        }
    }
}
