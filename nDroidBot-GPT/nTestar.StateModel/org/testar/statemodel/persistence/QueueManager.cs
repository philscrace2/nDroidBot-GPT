using System;
using System.Collections.Generic;
using org.testar.statemodel.events;
using org.testar.statemodel.exceptions;
using org.testar.statemodel.persistence.graphdb;
using org.testar.statemodel.sequence;
using org.testar.statemodel.util;

namespace org.testar.statemodel.persistence
{
    public class QueueManager : PersistenceManager, StateModelEventListener
    {
        private readonly Queue<Action> _queue;
        private readonly PersistenceManager _delegateManager;
        private readonly EventHelper _eventHelper;
        private bool _listening = true;
        private readonly bool _hybridMode;

        public QueueManager(PersistenceManager persistenceManager, EventHelper eventHelper, bool hybridMode)
        {
            _delegateManager = persistenceManager;
            _eventHelper = eventHelper;
            _hybridMode = hybridMode;
            _queue = new Queue<Action>();
        }

        private void ProcessRequest(Action action, Persistable persistable)
        {
            if (!_hybridMode || persistable.CanBeDelayed())
            {
                _queue.Enqueue(action);
            }
            else
            {
                action();
            }
        }

        public void Shutdown()
        {
            if (_queue.Count > 0)
            {
                int processed = 0;
                int total = _queue.Count;
                QueueVisualizer visualizer = new QueueVisualizer("Processing persistence queue");
                visualizer.UpdateMessage($"Processing persistence queue : {processed} / {total} processed");
                while (_queue.Count > 0)
                {
                    _queue.Dequeue().Invoke();
                    processed++;
                    visualizer.UpdateMessage($"Processing persistence queue : {processed} / {total} processed");
                }
                visualizer.Stop();
            }

            _delegateManager.Shutdown();
        }

        public void PersistAbstractState(AbstractState abstractState)
        {
            ProcessRequest(() => _delegateManager.PersistAbstractState(abstractState), abstractState);
        }

        public void PersistAbstractAction(AbstractAction abstractAction)
        {
            ProcessRequest(() => _delegateManager.PersistAbstractAction(abstractAction), abstractAction);
        }

        public void PersistAbstractStateTransition(AbstractStateTransition abstractStateTransition)
        {
            ProcessRequest(() => _delegateManager.PersistAbstractStateTransition(abstractStateTransition), abstractStateTransition);
        }

        public void PersistConcreteState(ConcreteState concreteState)
        {
            ProcessRequest(() => _delegateManager.PersistConcreteState(concreteState), concreteState);
        }

        public void PersistConcreteStateTransition(ConcreteStateTransition concreteStateTransition)
        {
            ProcessRequest(() => _delegateManager.PersistConcreteStateTransition(concreteStateTransition), concreteStateTransition);
        }

        public void InitAbstractStateModel(AbstractStateModel abstractStateModel)
        {
            SetListening(false);
            _delegateManager.InitAbstractStateModel(abstractStateModel);
            SetListening(true);
        }

        public void PersistSequence(Sequence sequence)
        {
            ProcessRequest(() => _delegateManager.PersistSequence(sequence), sequence);
        }

        public void InitSequenceManager(SequenceManager sequenceManager)
        {
            SetListening(false);
            _delegateManager.InitSequenceManager(sequenceManager);
            SetListening(true);
        }

        public void PersistSequenceNode(SequenceNode sequenceNode)
        {
            ProcessRequest(() => _delegateManager.PersistSequenceNode(sequenceNode), sequenceNode);
        }

        public void PersistSequenceStep(SequenceStep sequenceStep)
        {
            ProcessRequest(() => _delegateManager.PersistSequenceStep(sequenceStep), sequenceStep);
        }

        public bool ModelIsDeterministic(AbstractStateModel abstractStateModel)
        {
            return _delegateManager.ModelIsDeterministic(abstractStateModel);
        }

        public int GetNrOfNondeterministicActions(AbstractStateModel abstractStateModel)
        {
            return _delegateManager.GetNrOfNondeterministicActions(abstractStateModel);
        }

        public void EventReceived(StateModelEvent modelEvent)
        {
            if (!_listening)
            {
                return;
            }

            try
            {
                _eventHelper.ValidateEvent(modelEvent);
            }
            catch (InvalidEventException)
            {
                return;
            }

            switch (modelEvent.EventType)
            {
                case StateModelEventType.AbstractStateAdded:
                case StateModelEventType.AbstractStateChanged:
                    PersistAbstractState((AbstractState)modelEvent.Payload);
                    break;

                case StateModelEventType.AbstractStateTransitionAdded:
                case StateModelEventType.AbstractActionChanged:
                    PersistAbstractStateTransition((AbstractStateTransition)modelEvent.Payload);
                    break;

                case StateModelEventType.AbstractStateModelInitialized:
                    InitAbstractStateModel((AbstractStateModel)modelEvent.Payload);
                    break;

                case StateModelEventType.SequenceStarted:
                    PersistSequence((Sequence)modelEvent.Payload);
                    break;

                case StateModelEventType.SequenceManagerInitialized:
                    InitSequenceManager((SequenceManager)modelEvent.Payload);
                    break;

                case StateModelEventType.SequenceNodeAdded:
                    PersistSequenceNode((SequenceNode)modelEvent.Payload);
                    break;

                case StateModelEventType.SequenceStepAdded:
                    PersistSequenceStep((SequenceStep)modelEvent.Payload);
                    break;
            }
        }

        public void SetListening(bool listening)
        {
            _listening = listening;
        }

        public IGraphEntityManager GetEntityManager()
        {
            return _delegateManager.GetEntityManager();
        }
    }
}
