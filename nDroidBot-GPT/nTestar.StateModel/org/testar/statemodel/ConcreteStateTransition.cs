using System;
using org.testar.statemodel.persistence;

namespace org.testar.statemodel
{
    public class ConcreteStateTransition : Persistable
    {
        private readonly ConcreteState _sourceState;
        private readonly ConcreteState _targetState;
        private readonly ConcreteAction _action;

        public ConcreteStateTransition(ConcreteState sourceState, ConcreteState targetState, ConcreteAction action)
        {
            _sourceState = sourceState ?? throw new ArgumentNullException(nameof(sourceState), "Concrete source state cannot be null");
            _targetState = targetState ?? throw new ArgumentNullException(nameof(targetState), "Concrete target state cannot be null");
            _action = action ?? throw new ArgumentNullException(nameof(action), "ConcreteAction cannot be null");
        }

        public string GetSourceStateId()
        {
            return _sourceState.GetId();
        }

        public string GetTargetStateId()
        {
            return _targetState.GetId();
        }

        public string GetActionId()
        {
            return _action.GetActionId();
        }

        public ConcreteState GetSourceState()
        {
            return _sourceState;
        }

        public ConcreteState GetTargetState()
        {
            return _targetState;
        }

        public ConcreteAction GetAction()
        {
            return _action;
        }

        public bool CanBeDelayed()
        {
            return true;
        }
    }
}
