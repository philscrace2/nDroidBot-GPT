using System;
using System.Collections.Generic;
using org.testar.statemodel.events;
using org.testar.statemodel.persistence.graphdb;
using org.testar.statemodel.persistence.orientdb.entity;
using org.testar.statemodel.sequence;

namespace org.testar.statemodel.persistence.orientdb
{
    public class OrientDbManager : PersistenceManager, StateModelEventListener
    {
        private readonly EntityManager _entityManager;
        private readonly IGraphEntityManager _graphEntityManager;
        private bool _listening = true;

        public OrientDbManager(IGraphEntityManager graphEntityManager)
        {
            _graphEntityManager = graphEntityManager;
            _entityManager = new EntityManager(graphEntityManager);
        }

        public void Shutdown()
        {
            _entityManager.ReleaseConnection();
        }

        public void PersistAbstractState(AbstractState abstractState)
        {
            // TODO: Implement graph DB persistence mapping.
        }

        public void PersistAbstractAction(AbstractAction abstractAction)
        {
        }

        public void PersistAbstractStateTransition(AbstractStateTransition abstractStateTransition)
        {
        }

        public void PersistConcreteState(ConcreteState concreteState)
        {
        }

        public void PersistConcreteStateTransition(ConcreteStateTransition concreteStateTransition)
        {
        }

        public void InitAbstractStateModel(AbstractStateModel abstractStateModel)
        {
        }

        public void PersistSequence(Sequence sequence)
        {
        }

        public void InitSequenceManager(SequenceManager sequenceManager)
        {
        }

        public void PersistSequenceNode(SequenceNode sequenceNode)
        {
        }

        public void PersistSequenceStep(SequenceStep sequenceStep)
        {
        }

        public bool ModelIsDeterministic(AbstractStateModel abstractStateModel)
        {
            return true;
        }

        public int GetNrOfNondeterministicActions(AbstractStateModel abstractStateModel)
        {
            return 0;
        }

        public IGraphEntityManager GetEntityManager()
        {
            return _graphEntityManager;
        }

        public void EventReceived(StateModelEvent modelEvent)
        {
        }

        public void SetListening(bool listening)
        {
            _listening = listening;
        }

        
    }
}
