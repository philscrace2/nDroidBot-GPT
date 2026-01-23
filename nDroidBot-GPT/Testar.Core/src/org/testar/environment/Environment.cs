namespace org.testar.environment
{
    public static class Environment
    {
        private static IEnvironment? instance;

        public static IEnvironment GetInstance()
        {
            return instance ?? new UnknownEnvironment();
        }

        public static IEnvironment getInstance()
        {
            return GetInstance();
        }

        public static void SetInstance(IEnvironment implementation)
        {
            if (implementation == null)
            {
                throw new ArgumentException("Environment implementation cannot be set to null");
            }

            instance = implementation;
        }

        public static void setInstance(IEnvironment implementation)
        {
            SetInstance(implementation);
        }
    }
}
