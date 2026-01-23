using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.exceptions
{
    public class NoSuchTagException : Exception
    {
        public NoSuchTagException(ITag tag)
            : base($"No such tag: {tag}")
        {
        }
    }
}
