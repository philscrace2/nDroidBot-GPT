using System.Collections.Generic;

namespace org.testar.statemodel.persistence.graphdb
{
    public sealed class GraphQueryResult
    {
        public GraphQueryResult(IReadOnlyDictionary<string, object?> values)
        {
            Values = values;
        }

        public IReadOnlyDictionary<string, object?> Values { get; }
    }
}
