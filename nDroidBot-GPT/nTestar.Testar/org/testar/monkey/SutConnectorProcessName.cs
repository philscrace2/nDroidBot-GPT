using System.Diagnostics;
using org.testar.monkey.alayer;
using org.testar.monkey.alayer.devices;
using org.testar.monkey.alayer.exceptions;

namespace org.testar.monkey
{
    public class SutConnectorProcessName
    {
        private readonly string processName;
        private readonly long timeoutMs;

        public SutConnectorProcessName(string processName, long timeoutMs)
        {
            this.processName = processName;
            this.timeoutMs = timeoutMs;
        }

        public SUT startOrConnectSut()
        {
            string resolved = System.IO.Path.GetFileNameWithoutExtension(processName);
            Process? process = WaitForProcessByName(resolved, timeoutMs);
            if (process == null)
            {
                throw new SystemStartException($"Unable to connect to process name: {resolved}");
            }

            return new ConnectedSut(process, new AWTMouse(), new AWTKeyboard());
        }

        private static Process? WaitForProcessByName(string name, long timeoutMs)
        {
            DateTime deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs <= 0 ? 1 : timeoutMs);
            do
            {
                Process? process = Process.GetProcessesByName(name).FirstOrDefault();
                if (process != null)
                {
                    return process;
                }

                System.Threading.Thread.Sleep(200);
            } while (DateTime.UtcNow < deadline);

            return null;
        }
    }
}
