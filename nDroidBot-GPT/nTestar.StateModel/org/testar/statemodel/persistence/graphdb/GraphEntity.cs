using System.Collections.Generic;

namespace org.testar.statemodel.persistence.graphdb
{
    public abstract class GraphEntity
    {
        private readonly Dictionary<string, object?> _properties = new Dictionary<string, object?>();

        protected GraphEntity(GraphEntityClass entityClass)
        {
            EntityClass = entityClass;
        }

        public GraphEntityClass EntityClass { get; }

        public IReadOnlyDictionary<string, object?> Properties => _properties;

        public void SetProperty(string name, object? value)
        {
            _properties[name] = value;
        }

        public object? GetProperty(string name)
        {
            _properties.TryGetValue(name, out object? value);
            return value;
        }
    }
}
