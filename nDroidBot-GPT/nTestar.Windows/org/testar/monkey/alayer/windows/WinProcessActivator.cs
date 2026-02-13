using System.Runtime.InteropServices;

namespace org.testar.monkey.alayer.windows
{
    public static class WinProcessActivator
    {
        public static bool TryActivate(WinProcHandle handle, ActivateOptions? options = null)
        {
            if (!OperatingSystem.IsWindows() || !handle.IsValid)
            {
                return false;
            }

            options ??= ActivateOptions.Default;
            DateTime deadline = DateTime.UtcNow.AddMilliseconds(Math.Max(50, options.TimeoutMs));
            do
            {
                ShowWindow(handle.MainWindowHandle, SW_RESTORE);
                if (!options.ForceForeground || SetForegroundWindow(handle.MainWindowHandle))
                {
                    return true;
                }

                Thread.Sleep(50);
            } while (DateTime.UtcNow < deadline);

            return false;
        }

        private const int SW_RESTORE = 9;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}
