using System;
using System.Collections.Generic;

namespace org.testar.statemodel.persistence.orientdb.entity
{
    public sealed class TypeConvertor
    {
        private static readonly Lazy<TypeConvertor> LazyInstance = new(() => new TypeConvertor());
        private readonly Dictionary<Type, string> _typeMap;

        private TypeConvertor()
        {
            _typeMap = new Dictionary<Type, string>
            {
                [typeof(string)] = "STRING",
                [typeof(int)] = "INTEGER",
                [typeof(long)] = "LONG",
                [typeof(bool)] = "BOOLEAN",
                [typeof(double)] = "DOUBLE",
                [typeof(DateTime)] = "DATETIME",
                [typeof(DateTimeOffset)] = "DATETIME"
            };
        }

        public static TypeConvertor Instance => LazyInstance.Value;

        public string GetGraphType(Type type)
        {
            if (type == null)
            {
                return "ANY";
            }

            return _typeMap.TryGetValue(type, out string mapped) ? mapped : "ANY";
        }
    }
}
