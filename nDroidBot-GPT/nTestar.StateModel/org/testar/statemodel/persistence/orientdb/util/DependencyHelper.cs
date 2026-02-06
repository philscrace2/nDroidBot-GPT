using System.Collections.Generic;
using System.Linq;
using org.testar.statemodel.persistence.orientdb.entity;

namespace org.testar.statemodel.persistence.orientdb.util
{
    public static class DependencyHelper
    {
        public static IReadOnlyList<EntityClass> SortDependenciesForDeletion(IEnumerable<EntityClass> entityClasses)
        {
            if (entityClasses == null)
            {
                return new List<EntityClass>();
            }

            return entityClasses.OrderBy(c => c.SuperClassName == null ? 0 : 1).ToList();
        }
    }
}
