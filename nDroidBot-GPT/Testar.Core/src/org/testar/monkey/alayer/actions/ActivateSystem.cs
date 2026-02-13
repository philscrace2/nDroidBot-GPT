using System.Diagnostics;
using System.Runtime.InteropServices;
using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.actions
{
    [Serializable]
    public sealed class ActivateSystem : Action
    {
        public ActivateSystem()
        {
            set(Tags.Role, Roles.System);
            set(Tags.Desc, "Bring the system to the foreground.");
        }

        public override void run(SUT system, State state, double duration)
        {
            long pid = system.get(Tags.PID, 0L);
            if (pid <= 0 || !OperatingSystem.IsWindows())
            {
                return;
            }

            try
            {
                Process process = Process.GetProcessById((int)pid);
                IntPtr hwnd = process.MainWindowHandle;
                if (hwnd != IntPtr.Zero)
                {
                    SetForegroundWindow(hwnd);
                }
            }
            catch
            {
                // Best effort action.
            }
        }

        public override string toShortString()
        {
            return "ActivateSystem";
        }

        public override string toParametersString()
        {
            return string.Empty;
        }

        public override string toString(params Role[] discardParameters)
        {
            return ToString();
        }

        public override string ToString()
        {
            return "Activate system window";
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
