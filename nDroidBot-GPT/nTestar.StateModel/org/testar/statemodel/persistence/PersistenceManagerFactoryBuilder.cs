namespace org.testar.statemodel.persistence
{
    public static class PersistenceManagerFactoryBuilder
    {
        public enum ManagerType
        {
            OrientDb,
            Dummy
        }

        public static PersistenceManagerFactory CreatePersistenceManagerFactory(ManagerType managerType)
        {
            return managerType switch
            {
                ManagerType.OrientDb => new org.testar.statemodel.persistence.orientdb.OrientDbManagerFactory(),
                _ => new DummyManagerFactory(),
            };
        }
    }
}
