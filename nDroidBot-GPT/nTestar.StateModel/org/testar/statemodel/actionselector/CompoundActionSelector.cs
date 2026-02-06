using System.Collections.Generic;
using org.testar.statemodel.exceptions;

namespace org.testar.statemodel.actionselector
{
    public class CompoundActionSelector : ActionSelector
    {
        private readonly IReadOnlyList<ActionSelector> _selectors;

        public CompoundActionSelector(IReadOnlyList<ActionSelector> selectors)
        {
            _selectors = selectors;
        }

        public void NotifyNewSequence()
        {
            foreach (ActionSelector selector in _selectors)
            {
                selector.NotifyNewSequence();
            }
        }

        public AbstractAction SelectAction(AbstractState currentState, AbstractStateModel abstractStateModel)
        {
            foreach (ActionSelector selector in _selectors)
            {
                try
                {
                    return selector.SelectAction(currentState, abstractStateModel);
                }
                catch (ActionNotFoundException)
                {
                }
            }

            throw new ActionNotFoundException();
        }
    }
}
