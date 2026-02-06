namespace org.testar.statemodel.persistence.orientdb.entity
{
    public sealed class Connection
    {
        public Connection(string sourceId, string targetId, string actionId)
        {
            SourceId = sourceId;
            TargetId = targetId;
            ActionId = actionId;
        }

        public string SourceId { get; }

        public string TargetId { get; }

        public string ActionId { get; }
    }
}
