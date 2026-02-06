using System;
using System.Collections.Generic;
using org.testar.monkey.alayer;
using State = org.testar.monkey.alayer.State;
using Action = org.testar.monkey.alayer.Action;

namespace org.testar
{
    public class RandomActionSelector : IActionSelector
    {
        public static Action selectRandomActionUsingSystemTime(ISet<Action> actions)
        {
            long graphTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            Random rnd = new Random((int)(graphTime % int.MaxValue));
            var actionList = new List<Action>(actions);
            return actionList[rnd.Next(actionList.Count)];
        }

        public static Action selectRandomAction(ISet<Action> actions)
        {
            var actionList = new List<Action>(actions);
            int randomIndex = new Random().Next(actionList.Count);
            return actionList[randomIndex];
        }

        public Action? selectAction(State state, ISet<Action> actions)
        {
            return selectRandomAction(actions);
        }
    }
}
