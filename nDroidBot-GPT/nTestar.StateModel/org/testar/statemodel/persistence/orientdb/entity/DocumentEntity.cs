using org.testar.statemodel.persistence.graphdb;

namespace org.testar.statemodel.persistence.orientdb.entity
{
    public class DocumentEntity : GraphEntity
    {
        public DocumentEntity(EntityClass entityClass)
            : base(new GraphEntityClass(entityClass.ClassName, entityClass.SuperClassName))
        {
        }
    }
}
