using System.Diagnostics;

namespace org.testar.monkey.alayer.windows
{
    public sealed class WinProcess
    {
        public Process Process { get; }
        public WinProcHandle Handle => new(Process);

        public WinProcess(Process process)
        {
            Process = process;
        }

        public bool IsRunning => !Process.HasExited;

        public void Stop()
        {
            if (!Process.HasExited)
            {
                Process.Kill(true);
                Process.WaitForExit(2000);
            }
        }

        public static WinProcess Start(ProcessStartInfo startInfo)
        {
            Process process = Process.Start(startInfo)
                              ?? throw new InvalidOperationException($"Unable to start process: {startInfo.FileName}");
            return new WinProcess(process);
        }

        public static WinProcess Attach(int processId)
        {
            return new WinProcess(Process.GetProcessById(processId));
        }
    }
}
