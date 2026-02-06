namespace org.testar.statemodel.persistence.orientdb.entity
{
    public sealed class Property
    {
        public Property(string propertyName, string propertyType)
        {
            PropertyName = propertyName;
            PropertyType = propertyType;
        }

        public string PropertyName { get; }

        public string PropertyType { get; }
    }
}
