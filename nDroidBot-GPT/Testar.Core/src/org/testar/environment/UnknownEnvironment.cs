namespace org.testar.environment
{
    public class UnknownEnvironment : IEnvironment
    {
        public double GetDisplayScale(long windowHandle)
        {
            return 1.0;
        }
    }
}
