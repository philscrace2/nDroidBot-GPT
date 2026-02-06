using System;
using System.Collections.Generic;
using org.testar.statemodel.persistence.graphdb;

namespace org.testar.statemodel.persistence.orientdb.entity
{
    public class EntityManager
    {
        private readonly IGraphEntityManager _graphEntityManager;

        public EntityManager(IGraphEntityManager graphEntityManager)
        {
            _graphEntityManager = graphEntityManager;
        }

        public void CreateClass(EntityClass entityClass)
        {
            _graphEntityManager.CreateClass(new GraphEntityClass(entityClass.ClassName, entityClass.SuperClassName));
        }

        public void SaveEntity(GraphEntity entity)
        {
            _graphEntityManager.SaveEntity(entity);
        }

        public void DeleteEntities(EntityClass entityClass, IEnumerable<object> ids)
        {
            _graphEntityManager.DeleteEntities(new GraphEntityClass(entityClass.ClassName, entityClass.SuperClassName), ids);
        }

        public IEnumerable<GraphQueryResult> ExecuteQuery(string query, params object[] parameters)
        {
            return _graphEntityManager.ExecuteQuery(query, parameters);
        }

        public void ReleaseConnection()
        {
            _graphEntityManager.ReleaseConnection();
        }
    }
}
