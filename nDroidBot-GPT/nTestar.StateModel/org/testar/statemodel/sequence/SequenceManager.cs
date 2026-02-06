using System.Collections.Generic;
using org.testar.statemodel;
using org.testar.statemodel.events;

namespace org.testar.statemodel.sequence
{
    public class SequenceManager
    {
        private int _currentSequenceNr;
        private Sequence _currentSequence;
        private readonly string _modelIdentifier;
        private readonly ISet<StateModelEventListener> _eventListeners;

        public SequenceManager(ISet<StateModelEventListener> eventListeners, string modelIdentifier)
        {
            _eventListeners = eventListeners ?? new HashSet<StateModelEventListener>();
            _modelIdentifier = modelIdentifier;
            Init();
        }

        private void Init()
        {
            EmitEvent(new StateModelEvent(StateModelEventType.SequenceManagerInitialized, this));
        }

        private void EmitEvent(StateModelEvent modelEvent)
        {
            foreach (StateModelEventListener listener in _eventListeners)
            {
                listener.EventReceived(modelEvent);
            }
        }

        public void StartNewSequence()
        {
            if (_currentSequence != null && _currentSequence.IsRunning())
            {
                _currentSequence.Stop();
            }

            _currentSequence = new Sequence(++_currentSequenceNr, _eventListeners, _modelIdentifier);
            _currentSequence.Start();
        }

        public void StopSequence()
        {
            _currentSequence.SetSequenceVerdict(SequenceVerdict.CompletedSuccesfully);
            _currentSequence.Stop();
        }

        public void NotifyInterruptionByUser()
        {
            _currentSequence.SetSequenceVerdict(SequenceVerdict.InterruptedByUser);
            _currentSequence.Stop();
        }

        public void NotifyInterruptionBySystem(string message)
        {
            _currentSequence.SetSequenceVerdict(SequenceVerdict.InterruptedByError);
            _currentSequence.SetTerminationMessage(message);
            _currentSequence.Stop();
        }

        public void NotifyStateReached(ConcreteState concreteState, ConcreteAction concreteAction, params SequenceError[] sequenceErrors)
        {
            if (concreteState == null || _currentSequence == null || !_currentSequence.IsRunning())
            {
                return;
            }

            _currentSequence.AddNode(concreteState, concreteAction, sequenceErrors);
        }

        public void NotifyErrorInCurrentState(string errorMessage)
        {
            SequenceNode lastNode = _currentSequence?.GetLastNode();
            if (lastNode != null)
            {
                lastNode.AddErrorMessage(errorMessage);
            }
        }
    }
}
