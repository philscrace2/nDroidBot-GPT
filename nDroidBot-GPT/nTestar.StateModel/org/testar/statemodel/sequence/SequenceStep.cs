using System;
using org.testar.statemodel;
using org.testar.statemodel.persistence;

namespace org.testar.statemodel.sequence
{
    public class SequenceStep : Persistable
    {
        private readonly ConcreteAction _concreteAction;
        private readonly SequenceNode _sourceNode;
        private readonly SequenceNode _targetNode;
        private readonly DateTimeOffset _timestamp;
        private readonly string _actionDescription;
        private bool _nonDeterministic;

        public SequenceStep(ConcreteAction concreteAction, SequenceNode sourceNode, SequenceNode targetNode, string actionDescription)
        {
            _concreteAction = concreteAction;
            _sourceNode = sourceNode;
            _targetNode = targetNode;
            _actionDescription = actionDescription;
            _timestamp = DateTimeOffset.UtcNow;
            _nonDeterministic = false;
        }

        public ConcreteAction GetConcreteAction()
        {
            return _concreteAction;
        }

        public SequenceNode GetSourceNode()
        {
            return _sourceNode;
        }

        public SequenceNode GetTargetNode()
        {
            return _targetNode;
        }

        public DateTimeOffset GetTimestamp()
        {
            return _timestamp;
        }

        public bool CanBeDelayed()
        {
            return true;
        }

        public string GetActionDescription()
        {
            return _actionDescription;
        }

        public void SetNonDeterministic(bool nonDeterministic)
        {
            _nonDeterministic = nonDeterministic;
        }

        public bool IsNonDeterministic()
        {
            return _nonDeterministic;
        }
    }
}
