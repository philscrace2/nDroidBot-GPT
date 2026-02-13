namespace org.testar.monkey.alayer.windows
{
    public sealed class ActivateOptions
    {
        // Mirrors the Java ActivateOptions enum values used by UWP activation APIs.
        public enum ActivationMode
        {
            None = 0,
            DesignMode = 1,
            NoErrorUI = 2,
            NoSplashScreen = 3,
            Prelaunch = 33554432
        }

        public bool ForceForeground { get; init; } = true;
        public int TimeoutMs { get; init; } = 5000;
        public ActivationMode Mode { get; init; } = ActivationMode.None;

        public static ActivateOptions Default { get; } = new ActivateOptions();
    }
}
