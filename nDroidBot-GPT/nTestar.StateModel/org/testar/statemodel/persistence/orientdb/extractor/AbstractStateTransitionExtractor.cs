using org.testar.statemodel.persistence.orientdb.entity;

namespace org.testar.statemodel.persistence.orientdb.extractor
{
    public class AbstractStateTransitionExtractor : EntityExtractor<AbstractStateTransition>
    {
        public AbstractStateTransition Extract(DocumentEntity entity)
        {
            // TODO: Implement when graph DB backend is ready.
            var dummyState = new AbstractState(string.Empty, new System.Collections.Generic.HashSet<AbstractAction>());
            var dummyAction = new AbstractAction(string.Empty);
            return new AbstractStateTransition(dummyState, dummyState, dummyAction);
        }
    }
}
