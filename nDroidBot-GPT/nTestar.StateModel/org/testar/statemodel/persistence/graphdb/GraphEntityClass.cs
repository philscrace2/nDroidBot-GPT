namespace org.testar.statemodel.persistence.graphdb
{
    public sealed class GraphEntityClass
    {
        public GraphEntityClass(string className, string? superClassName = null)
        {
            ClassName = className;
            SuperClassName = superClassName;
        }

        public string ClassName { get; }

        public string? SuperClassName { get; }
    }
}
