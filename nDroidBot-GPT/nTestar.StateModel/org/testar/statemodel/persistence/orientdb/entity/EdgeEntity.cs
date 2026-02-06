using org.testar.statemodel.persistence.graphdb;

namespace org.testar.statemodel.persistence.orientdb.entity
{
    public class EdgeEntity : GraphEntity
    {
        public EdgeEntity(EntityClass entityClass, VertexEntity? outVertex = null, VertexEntity? inVertex = null)
            : base(new GraphEntityClass(entityClass.ClassName, entityClass.SuperClassName))
        {
            OutVertex = outVertex;
            InVertex = inVertex;
        }

        public VertexEntity? OutVertex { get; }

        public VertexEntity? InVertex { get; }

        public void EnableUpdate(bool allowUpdate)
        {
            // TODO: Implement when graph DB backend is selected.
        }
    }
}
