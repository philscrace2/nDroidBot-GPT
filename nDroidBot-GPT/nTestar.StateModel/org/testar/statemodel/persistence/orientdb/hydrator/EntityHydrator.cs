using org.testar.statemodel.persistence.orientdb.entity;
using org.testar.statemodel.persistence.graphdb;

namespace org.testar.statemodel.persistence.orientdb.hydrator
{
    public interface EntityHydrator
    {
        void Hydrate(GraphEntity entity, object modelObject);
    }
}
