using System.Collections.Generic;

namespace org.testar.statemodel.persistence.orientdb.entity
{
    public class EntityClass
    {
        public EntityClass(string className, string? superClassName = null)
        {
            ClassName = className;
            SuperClassName = superClassName;
            Properties = new HashSet<Property>();
        }

        public string ClassName { get; }

        public string? SuperClassName { get; }

        public ISet<Property> Properties { get; }
    }
}
