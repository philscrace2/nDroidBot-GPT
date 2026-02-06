using System.Collections.Generic;
using org.testar.monkey.alayer;

namespace org.testar
{
    public class ActionSelectorProxy : IActionSelector, IActionExecutor, IActionDerive
    {
        private readonly object selector;

        public ActionSelectorProxy(object selector)
        {
            this.selector = selector;
            System.Console.WriteLine("ActionSelector: " + selector.GetType().Name);
        }

        public ISet<Action> deriveActions(ISet<Action> actions)
        {
            if (selector is IActionDerive derive)
            {
                return derive.deriveActions(actions);
            }

            return actions;
        }

        public void executeAction(Action action)
        {
            if (selector is IActionExecutor executor)
            {
                executor.executeAction(action);
            }
        }

        public Action? selectAction(State state, ISet<Action> actions)
        {
            if (selector is IActionSelector actionSelector)
            {
                return actionSelector.selectAction(state, actions);
            }

            return null;
        }
    }
}
