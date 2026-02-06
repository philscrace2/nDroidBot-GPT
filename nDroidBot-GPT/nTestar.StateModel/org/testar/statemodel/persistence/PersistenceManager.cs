using org.testar.statemodel.persistence.graphdb;
using org.testar.statemodel.sequence;

namespace org.testar.statemodel.persistence
{
    public interface PersistenceManager
    {
        const string DataStoreModeInstant = "instant";
        const string DataStoreModeDelayed = "delayed";
        const string DataStoreModeHybrid = "hybrid";
        const string DataStoreModeNone = "none";

        void Shutdown();

        void PersistAbstractState(AbstractState abstractState);

        void PersistAbstractAction(AbstractAction abstractAction);

        void PersistAbstractStateTransition(AbstractStateTransition abstractStateTransition);

        void PersistConcreteState(ConcreteState concreteState);

        void PersistConcreteStateTransition(ConcreteStateTransition concreteStateTransition);

        void InitAbstractStateModel(AbstractStateModel abstractStateModel);

        void PersistSequence(Sequence sequence);

        void InitSequenceManager(SequenceManager sequenceManager);

        void PersistSequenceNode(SequenceNode sequenceNode);

        void PersistSequenceStep(SequenceStep sequenceStep);

        bool ModelIsDeterministic(AbstractStateModel abstractStateModel);

        int GetNrOfNondeterministicActions(AbstractStateModel abstractStateModel);

        IGraphEntityManager GetEntityManager();
    }
}
