using org.testar.statemodel.persistence.orientdb.entity;

namespace org.testar.statemodel.persistence.orientdb.hydrator
{
    public interface EntityHydrator
    {
        void Hydrate(GraphEntity entity, object modelObject);
    }
}
