using System;
using org.testar.statemodel.persistence;

namespace org.testar.statemodel
{
    public class AbstractStateTransition : Persistable
    {
        private readonly AbstractState _sourceState;
        private readonly AbstractState _targetState;
        private readonly AbstractAction _action;

        public AbstractStateTransition(AbstractState sourceState, AbstractState targetState, AbstractAction action)
        {
            _sourceState = sourceState ?? throw new ArgumentNullException(nameof(sourceState), "Abstract source state cannot be null");
            _targetState = targetState ?? throw new ArgumentNullException(nameof(targetState), "Abstract target state cannot be null");
            _action = action ?? throw new ArgumentNullException(nameof(action), "AbstractAction cannot be null");
        }

        public string GetSourceStateId()
        {
            return _sourceState.GetStateId();
        }

        public string GetTargetStateId()
        {
            return _targetState.GetStateId();
        }

        public string GetActionId()
        {
            return _action.GetActionId();
        }

        public AbstractState GetSourceState()
        {
            return _sourceState;
        }

        public AbstractState GetTargetState()
        {
            return _targetState;
        }

        public AbstractAction GetAction()
        {
            return _action;
        }

        public bool CanBeDelayed()
        {
            return false;
        }
    }
}
