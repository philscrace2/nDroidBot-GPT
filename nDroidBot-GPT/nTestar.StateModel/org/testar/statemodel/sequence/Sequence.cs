using System;
using System.Collections.Generic;
using org.testar.monkey.alayer;
using org.testar.statemodel;
using org.testar.statemodel.event;
using org.testar.statemodel.persistence;

namespace org.testar.statemodel.sequence
{
    public class Sequence : Persistable
    {
        private bool _active;
        private readonly string _currentSequenceId;
        private readonly int _currentSequenceNr;
        private int _currentNodeNr;
        private readonly List<SequenceNode> _nodes;
        private readonly string _modelIdentifier;
        private ISet<ITag> _concreteStateTags;
        private DateTimeOffset _startDateTime;
        private SequenceNode _currentNode;
        private SequenceVerdict _verdict;
        private string _terminationMessage;
        private readonly ISet<StateModelEventListener> _eventListeners;

        public Sequence(int currentSequenceNr, ISet<StateModelEventListener> eventListeners, string modelIdentifier)
        {
            _currentSequenceId = Guid.NewGuid().ToString();
            _eventListeners = eventListeners ?? new HashSet<StateModelEventListener>();
            _currentSequenceNr = currentSequenceNr;
            _modelIdentifier = modelIdentifier;
            _verdict = SequenceVerdict.CurrentlyExecuting;
            _nodes = new List<SequenceNode>();
        }

        public void Start()
        {
            _startDateTime = DateTimeOffset.UtcNow;
            _active = true;
            EmitEvent(new StateModelEvent(StateModelEventType.SequenceStarted, this));
        }

        private void EmitEvent(StateModelEvent modelEvent)
        {
            foreach (StateModelEventListener listener in _eventListeners)
            {
                listener.EventReceived(modelEvent);
            }
        }

        public string GetCurrentSequenceId()
        {
            return _currentSequenceId;
        }

        public int GetCurrentSequenceNr()
        {
            return _currentSequenceNr;
        }

        public string GetModelIdentifier()
        {
            return _modelIdentifier;
        }

        public ISet<ITag> GetConcreteStateTags()
        {
            return _concreteStateTags;
        }

        public DateTimeOffset GetStartDateTime()
        {
            return _startDateTime;
        }

        public bool IsRunning()
        {
            return _active;
        }

        public void Stop()
        {
            _active = false;
            EmitEvent(new StateModelEvent(StateModelEventType.SequenceEnded, this));
        }

        public void AddNode(ConcreteState concreteState, ConcreteAction concreteAction, params SequenceError[] sequenceErrors)
        {
            if (concreteAction == null)
            {
                AddFirstNode(concreteState);
            }
            else
            {
                AddStep(concreteState, concreteAction, sequenceErrors);
            }
        }

        private void AddFirstNode(ConcreteState concreteState)
        {
            SequenceNode node = new SequenceNode(_currentSequenceId, ++_currentNodeNr, concreteState, this, _eventListeners);
            _currentNode = node;
            _nodes.Add(node);
            EmitEvent(new StateModelEvent(StateModelEventType.SequenceNodeAdded, node));
        }

        private void AddStep(ConcreteState concreteState, ConcreteAction concreteAction, params SequenceError[] sequenceErrors)
        {
            SequenceNode targetNode = new SequenceNode(_currentSequenceId, ++_currentNodeNr, concreteState, null, _eventListeners);
            string actionDescription = concreteAction.GetAttributes().get(Tags.Desc, string.Empty);
            SequenceStep sequenceStep = new SequenceStep(concreteAction, _currentNode, targetNode, actionDescription);

            if (sequenceErrors != null)
            {
                foreach (SequenceError sequenceError in sequenceErrors)
                {
                    if (sequenceError == SequenceError.NonDeterministicAction)
                    {
                        sequenceStep.SetNonDeterministic(true);
                    }
                }
            }

            _nodes.Add(targetNode);
            _currentNode = targetNode;
            EmitEvent(new StateModelEvent(StateModelEventType.SequenceStepAdded, sequenceStep));
        }

        public bool CanBeDelayed()
        {
            return true;
        }

        public SequenceNode GetFirstNode()
        {
            return _nodes.Count > 0 ? _nodes[0] : null;
        }

        public SequenceNode GetLastNode()
        {
            return _nodes.Count > 0 ? _nodes[_nodes.Count - 1] : null;
        }

        public void SetSequenceVerdict(SequenceVerdict verdict)
        {
            _verdict = verdict;
        }

        public SequenceVerdict GetSequenceVerdict()
        {
            return _verdict;
        }

        public void SetTerminationMessage(string terminationMessage)
        {
            _terminationMessage = terminationMessage;
        }

        public string GetTerminationMessage()
        {
            return _terminationMessage;
        }
    }
}
