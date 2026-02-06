using System;
using System.Collections.Generic;
using System.Text;
using org.testar.statemodel;
using org.testar.statemodel.event;
using org.testar.statemodel.persistence;

namespace org.testar.statemodel.sequence
{
    public class SequenceNode : Persistable
    {
        private readonly DateTimeOffset _timestamp;
        private readonly string _nodeId;
        private readonly string _sequenceId;
        private readonly ConcreteState _concreteState;
        private readonly int _nodeNr;
        private readonly StringBuilder _errorMessages;
        private readonly Sequence _sequence;
        private readonly ISet<StateModelEventListener> _eventListeners;

        public SequenceNode(string sequenceId, int nodeNr, ConcreteState concreteState, Sequence sequence, ISet<StateModelEventListener> eventListeners)
        {
            _timestamp = DateTimeOffset.UtcNow;
            _nodeNr = nodeNr;
            _nodeId = sequenceId + "-" + nodeNr;
            _sequenceId = sequenceId;
            _concreteState = concreteState;
            _sequence = sequence;
            _errorMessages = new StringBuilder();
            _eventListeners = eventListeners ?? new HashSet<StateModelEventListener>();
        }

        public DateTimeOffset GetTimestamp()
        {
            return _timestamp;
        }

        public string GetNodeId()
        {
            return _nodeId;
        }

        public int GetNodeNr()
        {
            return _nodeNr;
        }

        public ConcreteState GetConcreteState()
        {
            return _concreteState;
        }

        public bool CanBeDelayed()
        {
            return true;
        }

        public bool IsFirstNode()
        {
            return _nodeNr == 1;
        }

        public string GetSequenceId()
        {
            return _sequenceId;
        }

        public Sequence GetSequence()
        {
            return _sequence;
        }

        public void AddErrorMessage(string message)
        {
            if (_errorMessages.Length > 0)
            {
                _errorMessages.Append(", ");
            }
            _errorMessages.Append(message);
            EmitEvent(new StateModelEvent(StateModelEventType.SequenceNodeUpdated, this));
        }

        public bool ContainsErrors()
        {
            return _errorMessages.Length > 0;
        }

        public string GetErrorMessage()
        {
            return _errorMessages.ToString();
        }

        private void EmitEvent(StateModelEvent modelEvent)
        {
            foreach (StateModelEventListener listener in _eventListeners)
            {
                listener.EventReceived(modelEvent);
            }
        }
    }
}
