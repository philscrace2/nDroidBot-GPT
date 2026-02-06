using org.testar.statemodel.events;
using org.testar.statemodel.persistence.graphdb;
using org.testar.statemodel.sequence;

namespace org.testar.statemodel.persistence
{
    public class DummyManager : PersistenceManager, StateModelEventListener
    {
        public void EventReceived(StateModelEvent modelEvent)
        {
        }

        public void SetListening(bool listening)
        {
        }

        public void Shutdown()
        {
        }

        public void PersistAbstractState(AbstractState abstractState)
        {
        }

        public void PersistAbstractAction(AbstractAction abstractAction)
        {
        }

        public void PersistAbstractStateTransition(AbstractStateTransition abstractStateTransition)
        {
        }

        public void InitAbstractStateModel(AbstractStateModel abstractStateModel)
        {
        }

        public void PersistConcreteState(ConcreteState concreteState)
        {
        }

        public void PersistConcreteStateTransition(ConcreteStateTransition concreteStateTransition)
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
            return null;
        }
    }
}
