using org.testar.statemodel.persistence.graphdb;

namespace org.testar.statemodel.persistence.orientdb.entity
{
    public class VertexEntity : GraphEntity
    {
        public VertexEntity(EntityClass entityClass)
            : base(new GraphEntityClass(entityClass.ClassName, entityClass.SuperClassName))
        {
        }

        public void EnableUpdate(bool allowUpdate)
        {
            // TODO: Implement when graph DB backend is selected.
        }
    }
}
