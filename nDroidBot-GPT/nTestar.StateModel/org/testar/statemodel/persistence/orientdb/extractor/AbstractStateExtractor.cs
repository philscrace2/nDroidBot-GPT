using org.testar.statemodel.persistence.orientdb.entity;

namespace org.testar.statemodel.persistence.orientdb.extractor
{
    public class AbstractStateExtractor : EntityExtractor<AbstractState>
    {
        public AbstractState Extract(DocumentEntity entity)
        {
            // TODO: Implement when graph DB backend is ready.
            return new AbstractState(string.Empty, new System.Collections.Generic.HashSet<AbstractAction>());
        }
    }
}
