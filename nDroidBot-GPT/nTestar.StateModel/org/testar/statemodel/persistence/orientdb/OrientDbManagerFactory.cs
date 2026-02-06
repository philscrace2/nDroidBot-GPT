using System;
using org.testar.monkey.alayer;
using org.testar.statemodel.persistence.graphdb;

namespace org.testar.statemodel.persistence.orientdb
{
    public class OrientDbManagerFactory : PersistenceManagerFactory
    {
        public PersistenceManager GetPersistenceManager(TaggableBase settings)
        {
            throw new NotImplementedException("TODO: Provide an IGraphEntityManager implementation for the graph DB backend.");
        }
    }
}
