using org.testar.monkey.alayer;

namespace org.testar.statemodel.persistence
{
    public interface PersistenceManagerFactory
    {
        PersistenceManager GetPersistenceManager(TaggableBase settings);
    }
}
