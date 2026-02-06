using System.Collections.Generic;

namespace org.testar.statemodel.persistence.graphdb
{
    public interface IGraphEntityManager
    {
        void CreateClass(GraphEntityClass entityClass);

        void SaveEntity(GraphEntity entity);

        void DeleteEntities(GraphEntityClass entityClass, IEnumerable<object> ids);

        IEnumerable<GraphQueryResult> ExecuteQuery(string query, params object[] parameters);

        void ReleaseConnection();
    }
}
