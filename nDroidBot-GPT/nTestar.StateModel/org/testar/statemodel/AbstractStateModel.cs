using System;
using System.Collections.Generic;
using org.testar.monkey.alayer;
using org.testar.statemodel.events;
using org.testar.statemodel.exceptions;

namespace org.testar.statemodel
{
    public class AbstractStateModel
    {
        private readonly string _modelIdentifier;
        private readonly string _applicationName;
        private readonly string _applicationVersion;
        private readonly ISet<ITag> _tags;

        private readonly HashSet<AbstractStateTransition> _stateTransitions;
        private readonly Dictionary<string, HashSet<AbstractStateTransition>> _stateTransitionsBySource;
        private readonly Dictionary<string, HashSet<AbstractStateTransition>> _stateTransitionsByTarget;
        private readonly Dictionary<string, AbstractState> _states;
        private readonly Dictionary<string, AbstractState> _initialStates;
        private readonly HashSet<StateModelEventListener> _eventListeners;
        private bool _emitEvents = true;

        public AbstractStateModel(
            string modelIdentifier,
            string applicationName,
            string applicationVersion,
            ISet<ITag> tags,
            params StateModelEventListener[] eventListeners)
        {
            if (modelIdentifier == null)
            {
                throw new ArgumentNullException(nameof(modelIdentifier), "Model identifier cannot be null");
            }

            if (string.IsNullOrWhiteSpace(modelIdentifier))
            {
                throw new ArgumentException("Model identifier cannot be empty or blank", nameof(modelIdentifier));
            }

            _modelIdentifier = modelIdentifier;
            _applicationName = applicationName ?? throw new ArgumentNullException(nameof(applicationName), "Application name cannot be null");
            _applicationVersion = applicationVersion ?? throw new ArgumentNullException(nameof(applicationVersion), "Application version cannot be null");
            _tags = new HashSet<ITag>(tags ?? throw new ArgumentNullException(nameof(tags), "AbstractStateModel Tags set cannot be null"));

            _stateTransitions = new HashSet<AbstractStateTransition>();
            _stateTransitionsBySource = new Dictionary<string, HashSet<AbstractStateTransition>>();
            _stateTransitionsByTarget = new Dictionary<string, HashSet<AbstractStateTransition>>();
            _states = new Dictionary<string, AbstractState>();
            _initialStates = new Dictionary<string, AbstractState>();
            _eventListeners = new HashSet<StateModelEventListener>();

            if (eventListeners != null)
            {
                foreach (StateModelEventListener listener in eventListeners)
                {
                    _eventListeners.Add(listener ?? throw new ArgumentNullException(nameof(eventListeners), "Event listener cannot be null"));
                }
            }

            InitStateModel();
        }

        private void InitStateModel()
        {
            EmitEvent(new StateModelEvent(StateModelEventType.AbstractStateModelInitialized, this));
        }

        public void AddTransition(AbstractState sourceState, AbstractState targetState, AbstractAction executedAction)
        {
            if (sourceState == null)
            {
                throw new ArgumentNullException(nameof(sourceState), "AbstractStateModel source state cannot be null");
            }

            if (targetState == null)
            {
                throw new ArgumentNullException(nameof(targetState), "AbstractStateModel target state cannot be null");
            }

            if (executedAction == null)
            {
                throw new ArgumentNullException(nameof(executedAction), "AbstractStateModel executed action cannot be null");
            }

            CheckStateId(sourceState.GetStateId());
            CheckStateId(targetState.GetStateId());

            if (_stateTransitionsBySource.TryGetValue(sourceState.GetStateId(), out HashSet<AbstractStateTransition> transitions))
            {
                foreach (AbstractStateTransition transition in transitions)
                {
                    if (targetState.GetStateId() == transition.GetTargetStateId() &&
                        executedAction.GetActionId() == transition.GetActionId())
                    {
                        EmitEvent(new StateModelEvent(StateModelEventType.AbstractStateTransitionChanged, transition));
                        return;
                    }
                }
            }

            sourceState.AddVisitedAction(executedAction);

            AbstractStateTransition newTransition = new AbstractStateTransition(sourceState, targetState, executedAction);
            DeactivateEvents();
            AddTransitionInternal(newTransition);
            AddState(sourceState);
            AddState(targetState);
            ActivateEvents();
            EmitEvent(new StateModelEvent(StateModelEventType.AbstractStateTransitionAdded, newTransition));
        }

        private void AddTransitionInternal(AbstractStateTransition newTransition)
        {
            _stateTransitions.Add(newTransition);

            if (!_stateTransitionsBySource.ContainsKey(newTransition.GetSourceStateId()))
            {
                _stateTransitionsBySource[newTransition.GetSourceStateId()] = new HashSet<AbstractStateTransition>();
            }
            _stateTransitionsBySource[newTransition.GetSourceStateId()].Add(newTransition);

            if (!_stateTransitionsByTarget.ContainsKey(newTransition.GetTargetStateId()))
            {
                _stateTransitionsByTarget[newTransition.GetTargetStateId()] = new HashSet<AbstractStateTransition>();
            }
            _stateTransitionsByTarget[newTransition.GetTargetStateId()].Add(newTransition);
        }

        public void AddState(AbstractState newState)
        {
            if (newState == null)
            {
                throw new ArgumentNullException(nameof(newState), "AbstractState cannot be null");
            }

            CheckStateId(newState.GetStateId());

            if (!ContainsState(newState.GetStateId()))
            {
                newState.SetModelIdentifier(_modelIdentifier);
                foreach (StateModelEventListener listener in _eventListeners)
                {
                    newState.AddEventListener(listener);
                }
                _states[newState.GetStateId()] = newState;
                EmitEvent(new StateModelEvent(StateModelEventType.AbstractStateAdded, newState));
            }
            else
            {
                EmitEvent(new StateModelEvent(StateModelEventType.AbstractStateChanged, newState));
            }

            if (newState.IsInitial())
            {
                AddInitialState(newState);
            }
        }

        public AbstractState GetState(string abstractStateId)
        {
            if (abstractStateId == null)
            {
                throw new ArgumentNullException(nameof(abstractStateId), "AbstractState ID cannot be null");
            }

            if (ContainsState(abstractStateId))
            {
                return _states[abstractStateId];
            }

            throw new StateNotFoundException();
        }

        public ISet<AbstractState> GetStates()
        {
            return new HashSet<AbstractState>(_states.Values);
        }

        public bool ContainsState(string abstractStateId)
        {
            if (abstractStateId == null)
            {
                throw new ArgumentNullException(nameof(abstractStateId), "AbstractState ID cannot be null");
            }

            return _states.ContainsKey(abstractStateId);
        }

        private void AddInitialState(AbstractState initialState)
        {
            CheckStateId(initialState.GetStateId());
            if (!_initialStates.ContainsKey(initialState.GetStateId()))
            {
                initialState.SetInitial(true);
                _initialStates[initialState.GetStateId()] = initialState;
            }
        }

        private void CheckStateId(string abstractStateId)
        {
            if (string.IsNullOrEmpty(abstractStateId))
            {
                throw new InvalidStateIdException();
            }
        }

        public ISet<AbstractStateTransition> GetOutgoingTransitionsForState(string stateId)
        {
            if (stateId == null)
            {
                throw new ArgumentNullException(nameof(stateId), "AbstractState ID cannot be null");
            }

            return _stateTransitionsBySource.TryGetValue(stateId, out HashSet<AbstractStateTransition> transitions)
                ? transitions
                : new HashSet<AbstractStateTransition>();
        }

        public ISet<AbstractStateTransition> GetIncomingTransitionsForState(string stateId)
        {
            if (stateId == null)
            {
                throw new ArgumentNullException(nameof(stateId), "AbstractState ID cannot be null");
            }

            return _stateTransitionsByTarget.TryGetValue(stateId, out HashSet<AbstractStateTransition> transitions)
                ? transitions
                : new HashSet<AbstractStateTransition>();
        }

        public void AddEventListener(StateModelEventListener eventListener)
        {
            if (eventListener == null)
            {
                throw new ArgumentNullException(nameof(eventListener), "StateModelEventListener cannot be null");
            }

            _eventListeners.Add(eventListener);
        }

        private void EmitEvent(StateModelEvent modelEvent)
        {
            if (!_emitEvents)
            {
                return;
            }

            foreach (StateModelEventListener listener in _eventListeners)
            {
                listener.EventReceived(modelEvent);
            }
        }

        public string GetModelIdentifier()
        {
            return _modelIdentifier;
        }

        public ISet<ITag> GetTags()
        {
            return _tags;
        }

        private void DeactivateEvents()
        {
            _emitEvents = false;
        }

        private void ActivateEvents()
        {
            _emitEvents = true;
        }

        public string GetApplicationName()
        {
            return _applicationName;
        }

        public string GetApplicationVersion()
        {
            return _applicationVersion;
        }
    }
}
