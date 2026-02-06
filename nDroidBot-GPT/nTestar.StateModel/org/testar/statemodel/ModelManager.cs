using System;
using System.Collections.Generic;
using System.Linq;
using org.testar.monkey.alayer;
using org.testar.monkey.alayer.exceptions;
using org.testar.statemodel.actionselector;
using org.testar.statemodel.exceptions;
using org.testar.statemodel.persistence;
using org.testar.statemodel.persistence.graphdb;
using org.testar.statemodel.sequence;
using org.testar.statemodel.util;
using Action = org.testar.monkey.alayer.Action;

namespace org.testar.statemodel
{
    public class ModelManager : StateModelManager
    {
        private readonly AbstractStateModel _abstractStateModel;
        private AbstractState _currentAbstractState;
        private AbstractAction _actionUnderExecution;
        private readonly ActionSelector _actionSelector;
        private readonly PersistenceManager _persistenceManager;
        private ConcreteState _currentConcreteState;
        private ConcreteAction _concreteActionUnderExecution;
        private readonly SequenceManager _sequenceManager;
        private List<string> _errorMessages;
        private int _nrOfNonDeterministicActions;
        private readonly bool _storeWidgets;

        public ModelManager(
            AbstractStateModel abstractStateModel,
            ActionSelector actionSelector,
            PersistenceManager persistenceManager,
            SequenceManager sequenceManager,
            bool storeWidgets)
        {
            _abstractStateModel = abstractStateModel;
            _actionSelector = actionSelector;
            _persistenceManager = persistenceManager;
            _sequenceManager = sequenceManager;
            _errorMessages = new List<string>();
            _nrOfNonDeterministicActions = 0;
            _storeWidgets = storeWidgets;
            Init();
        }

        private void Init()
        {
            bool modelIsDeterministic = _persistenceManager.ModelIsDeterministic(_abstractStateModel);
            Console.WriteLine("Model is deterministic: " + _persistenceManager.ModelIsDeterministic(_abstractStateModel));
            if (!modelIsDeterministic)
            {
                _nrOfNonDeterministicActions = _persistenceManager.GetNrOfNondeterministicActions(_abstractStateModel);
            }
        }

        public void NotifyNewStateReached(State newState, HashSet<Action> actions)
        {
            string abstractStateId = newState.get(Tags.AbstractID);
            AbstractState newAbstractState;

            if (_abstractStateModel.ContainsState(abstractStateId))
            {
                try
                {
                    newAbstractState = _abstractStateModel.GetState(abstractStateId);
                    AbstractStateService.UpdateAbstractStateActions(newAbstractState, actions);
                }
                catch (StateModelException ex)
                {
                    Console.WriteLine(ex);
                    throw new InvalidOperationException("An error occurred while retrieving abstract state from the state model");
                }
            }
            else
            {
                newAbstractState = AbstractStateFactory.CreateAbstractState(newState, actions);
            }

            newAbstractState.AddConcreteStateId(newState.get(Tags.ConcreteID));

            if (_actionUnderExecution == null)
            {
                newAbstractState.SetInitial(true);
                try
                {
                    _abstractStateModel.AddState(newAbstractState);
                }
                catch (StateModelException ex)
                {
                    Console.WriteLine(ex);
                    throw new InvalidOperationException("An error occurred while adding a new abstract state to the model");
                }
            }
            else
            {
                if (_currentAbstractState == null)
                {
                    throw new InvalidOperationException("An action was being executed without a recorded current state");
                }

                try
                {
                    _abstractStateModel.AddTransition(_currentAbstractState, newAbstractState, _actionUnderExecution);
                }
                catch (StateModelException ex)
                {
                    Console.WriteLine(ex);
                    throw new InvalidOperationException("Encountered a problem adding a state transition into the statemodel");
                }

                _actionUnderExecution = null;
            }

            _currentAbstractState = newAbstractState;

            ConcreteState newConcreteState = ConcreteStateFactory.CreateConcreteState(newState, newAbstractState, _storeWidgets);
            if (_concreteActionUnderExecution == null)
            {
                _persistenceManager.PersistConcreteState(newConcreteState);
            }
            else
            {
                ConcreteStateTransition concreteStateTransition = new ConcreteStateTransition(_currentConcreteState, newConcreteState, _concreteActionUnderExecution);
                _persistenceManager.PersistConcreteStateTransition(concreteStateTransition);
            }

            int currentNrOfNonDeterministicActions = _persistenceManager.GetNrOfNondeterministicActions(_abstractStateModel);
            if (currentNrOfNonDeterministicActions > _nrOfNonDeterministicActions)
            {
                Console.WriteLine("Non-deterministic action was executed!");
                _sequenceManager.NotifyStateReached(newConcreteState, _concreteActionUnderExecution, SequenceError.NonDeterministicAction);
                _nrOfNonDeterministicActions = currentNrOfNonDeterministicActions;
            }
            else
            {
                _sequenceManager.NotifyStateReached(newConcreteState, _concreteActionUnderExecution);
            }

            _currentConcreteState = newConcreteState;
            _concreteActionUnderExecution = null;

            Console.WriteLine(_abstractStateModel.GetStates().Count + " abstract states in the model");

            long unvisitedCount = _abstractStateModel.GetStates()
                .SelectMany(state => state.GetUnvisitedActions())
                .LongCount();
            Console.WriteLine(unvisitedCount + " unvisited actions left");
            Console.WriteLine("----------------------------");
            Console.WriteLine();
        }

        public void NotifyActionExecution(Action action)
        {
            try
            {
                _actionUnderExecution = _currentAbstractState.GetAction(action.get(Tags.AbstractID));
            }
            catch (ActionNotFoundException)
            {
                Console.WriteLine("Action not found in state model");
                _errorMessages.Add("Action with id: " + action.get(Tags.AbstractID) + " was not found in the model.");
                _actionUnderExecution = new AbstractAction(action.get(Tags.AbstractID));
                _currentAbstractState.AddNewAction(_actionUnderExecution);
            }

            _concreteActionUnderExecution = ConcreteActionFactory.CreateConcreteAction(action, _actionUnderExecution);
            _actionUnderExecution.AddConcreteActionId(_concreteActionUnderExecution.GetActionId());
            Console.WriteLine("Executing action: " + action.get(Tags.Desc));
            Console.WriteLine("----------------------------------");

            if (_errorMessages.Count > 0)
            {
                _sequenceManager.NotifyErrorInCurrentState(string.Join(", ", _errorMessages));
                _errorMessages = new List<string>();
            }
        }

        public void NotifyTestingEnded()
        {
            _persistenceManager.Shutdown();
        }

        public Action GetAbstractActionToExecute(HashSet<Action> actions)
        {
            if (_currentAbstractState == null)
            {
                return null;
            }

            try
            {
                string abstractId = _actionSelector.SelectAction(_currentAbstractState, _abstractStateModel).GetActionId();
                Console.WriteLine("Finding action with abstractId : " + abstractId);
                foreach (Action action in actions)
                {
                    try
                    {
                        if (action.get(Tags.AbstractID) == abstractId)
                        {
                            return action;
                        }
                    }
                    catch (NoSuchTagException)
                    {
                        string message = "ERROR getAbstractActionToExecute : " + action.get(Tags.Desc, "No description");
                        Console.WriteLine(message);
                        _errorMessages.Add(message);
                    }
                }

                Console.WriteLine("Could not find action with abstractId : " + abstractId);
                _errorMessages.Add("The actions selector returned the action with abstractId: " + abstractId + " . However, TESTAR was unable to find the action in its executable actions");
            }
            catch (ActionNotFoundException)
            {
                Console.WriteLine("Could not find an action to execute for abstract state id : " + _currentAbstractState.GetStateId());
            }

            return null;
        }

        public void NotifyTestSequencedStarted()
        {
            _sequenceManager.StartNewSequence();
            _actionSelector.NotifyNewSequence();
        }

        public void NotifyTestSequenceStopped()
        {
            _currentAbstractState = null;
            _currentConcreteState = null;
            _actionUnderExecution = null;
            _concreteActionUnderExecution = null;
            _sequenceManager.StopSequence();
        }

        public void NotifyTestSequenceInterruptedByUser()
        {
            _sequenceManager.NotifyInterruptionByUser();
        }

        public void NotifyTestSequenceInterruptedBySystem(string message)
        {
            _sequenceManager.NotifyInterruptionBySystem(message);
        }

        public string GetModelIdentifier()
        {
            return _abstractStateModel.GetModelIdentifier();
        }

        public string QueryStateModel(string query, params object[] parameters)
        {
            IGraphEntityManager manager = _persistenceManager.GetEntityManager();
            if (manager == null)
            {
                return "Empty";
            }

            try
            {
                foreach (GraphQueryResult result in manager.ExecuteQuery(query, parameters))
                {
                    return string.Join(", ", result.Values.Select(kv => $"{kv.Key}={kv.Value}"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return "Empty";
        }
    }
}
