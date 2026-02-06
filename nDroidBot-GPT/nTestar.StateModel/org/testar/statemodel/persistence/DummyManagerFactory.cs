using org.testar.monkey.alayer;

namespace org.testar.statemodel.persistence
{
    public class DummyManagerFactory : PersistenceManagerFactory
    {
        public PersistenceManager GetPersistenceManager(TaggableBase settings)
        {
            return new DummyManager();
        }
    }
}
