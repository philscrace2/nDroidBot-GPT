namespace org.testar.monkey
{
    public class ConfigException : System.Exception
    {
        public ConfigException(string message, System.Exception? inner = null) : base(message, inner)
        {
        }
    }
}
