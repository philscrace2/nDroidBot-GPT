using System;
using System.Collections.Generic;
using org.testar.statemodel.persistence;

namespace org.testar.statemodel
{
    public class AbstractAction : AbstractEntity, Persistable
    {
        private readonly HashSet<string> _concreteActionIds;

        public AbstractAction(string actionId)
            : base(actionId ?? throw new ArgumentNullException(nameof(actionId), "AbstractAction ID cannot be null"))
        {
            if (string.IsNullOrWhiteSpace(actionId))
            {
                throw new ArgumentException("AbstractAction ID cannot be empty or blank", nameof(actionId));
            }

            _concreteActionIds = new HashSet<string>();
        }

        public string GetActionId()
        {
            return GetId();
        }

        public void AddConcreteActionId(string concreteActionId)
        {
            if (concreteActionId == null)
            {
                throw new ArgumentNullException(nameof(concreteActionId), "ConcreteAction ID cannot be null");
            }

            if (string.IsNullOrWhiteSpace(concreteActionId))
            {
                throw new ArgumentException("ConcreteAction ID cannot be empty or blank", nameof(concreteActionId));
            }

            _concreteActionIds.Add(concreteActionId);
        }

        public IReadOnlyCollection<string> GetConcreteActionIds()
        {
            return _concreteActionIds;
        }

        public bool CanBeDelayed()
        {
            return false;
        }
    }
}
