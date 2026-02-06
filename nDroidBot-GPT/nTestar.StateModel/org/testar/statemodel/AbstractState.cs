using System;
using System.Collections.Generic;
using System.Linq;
using org.testar.statemodel.exceptions;
using org.testar.statemodel.persistence;

namespace org.testar.statemodel
{
    public class AbstractState : AbstractEntity, Persistable
    {
        private readonly Dictionary<string, AbstractAction> _actions;
        private readonly Dictionary<string, AbstractAction> _unvisitedActions;
        private readonly Dictionary<string, AbstractAction> _visitedActions;
        private readonly HashSet<string> _concreteStateIds;
        private bool _isInitial;

        public AbstractState(string stateId, ISet<AbstractAction> actions)
            : base(stateId ?? throw new ArgumentNullException(nameof(stateId), "AbstractState ID cannot be null"))
        {
            if (string.IsNullOrWhiteSpace(stateId))
            {
                throw new ArgumentException("AbstractState ID cannot be empty or blank", nameof(stateId));
            }

            _actions = new Dictionary<string, AbstractAction>();
            _unvisitedActions = new Dictionary<string, AbstractAction>();
            _visitedActions = new Dictionary<string, AbstractAction>();

            if (actions != null)
            {
                foreach (AbstractAction action in actions)
                {
                    if (action == null)
                    {
                        throw new ArgumentNullException(nameof(actions), "AbstractAction in actions set cannot be null");
                    }

                    _actions[action.GetActionId()] = action;
                    _unvisitedActions[action.GetActionId()] = action;
                }
            }

            _concreteStateIds = new HashSet<string>();
        }

        public void AddConcreteStateId(string concreteStateId)
        {
            if (concreteStateId == null)
            {
                throw new ArgumentNullException(nameof(concreteStateId), "ConcreteState ID cannot be null");
            }

            if (string.IsNullOrWhiteSpace(concreteStateId))
            {
                throw new ArgumentException("ConcreteState ID cannot be empty or blank", nameof(concreteStateId));
            }

            _concreteStateIds.Add(concreteStateId);
        }

        public string GetStateId()
        {
            return GetId();
        }

        public void AddVisitedAction(AbstractAction action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action), "Visited AbstractAction cannot be null");
            }

            _unvisitedActions.Remove(action.GetActionId());
            _visitedActions[action.GetActionId()] = action;
        }

        public IReadOnlyCollection<string> GetActionIds()
        {
            return _actions.Keys.ToList();
        }

        public IReadOnlyCollection<AbstractAction> GetActions()
        {
            return _actions.Values.ToList();
        }

        public AbstractAction GetAction(string actionId)
        {
            if (actionId == null)
            {
                throw new ArgumentNullException(nameof(actionId), "AbstractAction ID cannot be null");
            }

            if (string.IsNullOrWhiteSpace(actionId))
            {
                throw new ArgumentException("AbstractAction ID cannot be empty or blank", nameof(actionId));
            }

            if (!_actions.TryGetValue(actionId, out AbstractAction action))
            {
                throw new ActionNotFoundException();
            }

            return action;
        }

        public IReadOnlyCollection<string> GetConcreteStateIds()
        {
            return _concreteStateIds;
        }

        public IReadOnlyCollection<AbstractAction> GetUnvisitedActions()
        {
            return _unvisitedActions.Values.ToList();
        }

        public IReadOnlyCollection<AbstractAction> GetVisitedActions()
        {
            return _visitedActions.Values.ToList();
        }

        public bool IsInitial()
        {
            return _isInitial;
        }

        public void SetInitial(bool initial)
        {
            _isInitial = initial;
        }

        public override void SetModelIdentifier(string modelIdentifier)
        {
            base.SetModelIdentifier(modelIdentifier);
            foreach (string key in _actions.Keys)
            {
                _actions[key].SetModelIdentifier(modelIdentifier);
                if (_unvisitedActions.ContainsKey(key))
                {
                    _unvisitedActions[key].SetModelIdentifier(modelIdentifier);
                }
            }
        }

        public void AddNewAction(AbstractAction action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action), "Added AbstractAction cannot be null");
            }

            if (!_actions.ContainsKey(action.GetActionId()))
            {
                action.SetModelIdentifier(GetModelIdentifier());
                _actions[action.GetActionId()] = action;
                _unvisitedActions[action.GetActionId()] = action;
            }
        }

        public bool CanBeDelayed()
        {
            return false;
        }
    }
}
