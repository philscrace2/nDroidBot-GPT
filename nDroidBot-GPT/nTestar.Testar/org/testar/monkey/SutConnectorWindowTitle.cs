using System.Diagnostics;
using org.testar.monkey.alayer;
using org.testar.monkey.alayer.devices;
using org.testar.monkey.alayer.exceptions;

namespace org.testar.monkey
{
    public class SutConnectorWindowTitle
    {
        private readonly string windowTitle;
        private readonly long timeoutMs;
        private readonly StateBuilder? builder;
        private readonly bool forceForeground;

        public SutConnectorWindowTitle(string windowTitle, long timeoutMs, StateBuilder? builder, bool forceForeground)
        {
            this.windowTitle = windowTitle;
            this.timeoutMs = timeoutMs;
            this.builder = builder;
            this.forceForeground = forceForeground;
        }

        public SUT startOrConnectSut()
        {
            Process? process = WaitForWindowTitle(windowTitle, timeoutMs);
            if (process == null)
            {
                throw new SystemStartException($"Unable to connect to window title: {windowTitle}");
            }

            if (process != null && forceForeground)
            {
                TrySetForeground(process);
            }

            return new ConnectedSut(process, new AWTMouse(), new AWTKeyboard());
        }

        private static Process? WaitForWindowTitle(string title, long timeoutMs)
        {
            DateTime deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs <= 0 ? 1 : timeoutMs);
            do
            {
                Process? process = Process.GetProcesses().FirstOrDefault(p =>
                {
                    try
                    {
                        return !string.IsNullOrWhiteSpace(p.MainWindowTitle) &&
                               p.MainWindowTitle.Contains(title, StringComparison.OrdinalIgnoreCase);
                    }
                    catch
                    {
                        return false;
                    }
                });

                if (process != null)
                {
                    return process;
                }

                System.Threading.Thread.Sleep(200);
            } while (DateTime.UtcNow < deadline);

            return null;
        }

        private static void TrySetForeground(Process process)
        {
            IntPtr handle = process.MainWindowHandle;
            if (handle == IntPtr.Zero)
            {
                return;
            }

            _ = SetForegroundWindow(handle);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
