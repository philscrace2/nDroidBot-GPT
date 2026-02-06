using org.testar.statemodel.persistence.orientdb.entity;

namespace org.testar.statemodel.persistence.orientdb.extractor
{
    public class AbstractActionExtractor : EntityExtractor<AbstractAction>
    {
        public AbstractAction Extract(DocumentEntity entity)
        {
            // TODO: Implement when graph DB backend is ready.
            return new AbstractAction(string.Empty);
        }
    }
}
