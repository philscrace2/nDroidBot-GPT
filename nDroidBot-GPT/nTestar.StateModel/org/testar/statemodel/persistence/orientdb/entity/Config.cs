namespace org.testar.statemodel.persistence.orientdb.entity
{
    public class Config
    {
        public const string ConnectionTypeRemote = "remote";
        public const string ConnectionTypeLocal = "plocal";

        public string ConnectionType { get; set; } = ConnectionTypeRemote;

        public string Server { get; set; } = string.Empty;

        public string DatabaseDirectory { get; set; } = string.Empty;

        public string Database { get; set; } = string.Empty;

        public string User { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
