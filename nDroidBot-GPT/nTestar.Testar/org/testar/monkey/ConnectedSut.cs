using System.Diagnostics;
using org.testar.monkey.alayer;
using org.testar.monkey.alayer.devices;
using org.testar.serialisation;

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

            // Only use actual SUT process, not TESTAR's fallback
            if (process != null)
            {
                long pid = process.Id;
                set(Tags.PID, pid);
                LogSerialiser.Log($"ConnectedSut.AttachProcess: PID={pid}, ProcessName={process.ProcessName}{Environment.NewLine}", LogSerialiser.LogLevel.Info);

                // Set HWND for the SUT process (critical for StateFetcher)
                if (OperatingSystem.IsWindows())
                {
                    try
                    {
                        IntPtr mainWindowHandle = TryGetMainWindowHandle(process);

                        if (mainWindowHandle != IntPtr.Zero)
                        {
                            long hwnd = mainWindowHandle.ToInt64();
                            set(Tags.HWND, hwnd);
                            LogSerialiser.Log($"ConnectedSut.AttachProcess: Set Tags.HWND=0x{hwnd:X}{Environment.NewLine}", LogSerialiser.LogLevel.Info);
                        }
                        else
                        {
                            LogSerialiser.Log($"ConnectedSut.AttachProcess: MainWindowHandle still Zero after retries - window not created yet{Environment.NewLine}", LogSerialiser.LogLevel.Critical);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogSerialiser.Log($"ConnectedSut.AttachProcess: Exception getting HWND: {ex.Message}{Environment.NewLine}", LogSerialiser.LogLevel.Critical);
                    }
                }
            }
            else
            {
                // Fallback case - no actual SUT process attached
                LogSerialiser.Log($"ConnectedSut.AttachProcess: No process attached, using fallback PID={currentProcessId}{Environment.NewLine}", LogSerialiser.LogLevel.Critical);
                set(Tags.PID, (long)currentProcessId);
            }

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
                    // A process can be alive but not yet have a UI message loop/main window.
                    // Treat this as "unknown" instead of "not responding" to avoid early loop exit.
                    if (process.MainWindowHandle == IntPtr.Zero)
                    {
                        return false;
                    }

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

        private static IntPtr TryGetMainWindowHandle(Process process)
        {
            const int maxAttempts = 10;
            const int delayMs = 100;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                try
                {
                    process.Refresh();
                    IntPtr handle = process.MainWindowHandle;

                    if (handle != IntPtr.Zero)
                    {
                        return handle;
                    }

                    if (attempt < maxAttempts - 1)
                    {
                        System.Threading.Thread.Sleep(delayMs);
                    }
                }
                catch
                {
                    return IntPtr.Zero;
                }
            }

            return IntPtr.Zero;
        }
    }
}
