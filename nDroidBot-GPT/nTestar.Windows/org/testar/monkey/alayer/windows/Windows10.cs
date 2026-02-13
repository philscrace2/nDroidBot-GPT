namespace org.testar.monkey.alayer.windows
{
    public static class Windows10
    {
        public static Version? Version => Environment.OSVersion.Version;
        public static bool IsWindows10OrLater => OperatingSystem.IsWindowsVersionAtLeast(10);

        public static bool IsAtLeastBuild(int build)
        {
            return IsWindows10OrLater && Version != null && Version.Build >= build;
        }
    }
}
