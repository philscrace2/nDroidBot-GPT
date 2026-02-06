using System;

namespace org.testar.statemodel
{
    public class ConcreteAction : ModelWidget
    {
        private readonly string _actionId;
        private readonly AbstractAction _abstractAction;

        public ConcreteAction(string actionId, AbstractAction abstractAction)
            : base(actionId ?? throw new ArgumentNullException(nameof(actionId), "ConcreteAction ID cannot be null"))
        {
            if (string.IsNullOrWhiteSpace(actionId))
            {
                throw new ArgumentException("ConcreteAction ID cannot be empty or blank", nameof(actionId));
            }

            _actionId = actionId;
            _abstractAction = abstractAction ?? throw new ArgumentNullException(nameof(abstractAction), "AbstractAction cannot be null");
        }

        public string GetActionId()
        {
            return _actionId;
        }

        public AbstractAction GetAbstractAction()
        {
            return _abstractAction;
        }
    }
}
