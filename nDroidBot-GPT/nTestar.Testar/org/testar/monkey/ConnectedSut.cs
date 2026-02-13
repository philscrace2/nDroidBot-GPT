using System.Diagnostics;
using org.testar.monkey.alayer;
using org.testar.monkey.alayer.devices;

namespace org.testar.monkey
{
    public class ConnectedSut : TaggableBase, SUT
    {
        private readonly int currentProcessId = Environment.ProcessId;
        private Process? process;

        public ConnectedSut(Process? runningProcess, Mouse? mouse = null, Keyboard? keyboard = null)
        {
            AttachProcess(runningProcess);
            if (mouse != null)
            {
                set(Tags.StandardMouse, mouse);
                set(Tags.HasStandardMouse, true);
            }

            if (keyboard != null)
            {
                set(Tags.StandardKeyboard, keyboard);
            }
        }

        public void AttachProcess(Process? runningProcess)
        {
            process = runningProcess;
            long pid = process?.Id ?? currentProcessId;
            set(Tags.PID, pid);
            set(Tags.SystemActivator, true);
            set(Tags.IsRunning, process == null || !process.HasExited);
        }

        public bool IsRunning => process == null || !process.HasExited;

        public bool IsNotResponding
        {
            get
            {
                if (process == null || process.HasExited || !OperatingSystem.IsWindows())
                {
                    return false;
                }

                try
                {
                    return !process.Responding;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool IsForeground
        {
            get
            {
                if (!OperatingSystem.IsWindows())
                {
                    return true;
                }

                int pid = process?.Id ?? currentProcessId;
                return IsForegroundProcess(pid);
            }
        }

        public void Stop()
        {
            if (process == null)
            {
                return;
            }

            try
            {
                if (!process.HasExited)
                {
                    process.Kill(true);
                    process.WaitForExit(2000);
                }
            }
            catch
            {
                // Best effort.
            }
        }

        public Process? AttachedProcess => process;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        private static bool IsForegroundProcess(int pid)
        {
            IntPtr hwnd = GetForegroundWindow();
            if (hwnd == IntPtr.Zero)
            {
                return true;
            }

            _ = GetWindowThreadProcessId(hwnd, out uint foregroundPid);
            return foregroundPid == pid;
        }
    }
}
