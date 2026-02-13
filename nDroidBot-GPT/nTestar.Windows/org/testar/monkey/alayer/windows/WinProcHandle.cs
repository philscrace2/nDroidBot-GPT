using System.Diagnostics;

namespace org.testar.monkey.alayer.windows
{
    public sealed class WinProcHandle
    {
        public int ProcessId { get; }
        public IntPtr MainWindowHandle { get; }
        public bool IsValid => ProcessId > 0 && MainWindowHandle != IntPtr.Zero;

        public WinProcHandle(Process process)
        {
            ProcessId = process.Id;
            MainWindowHandle = process.MainWindowHandle;
        }

        public WinProcHandle(int processId, IntPtr mainWindowHandle)
        {
            ProcessId = processId;
            MainWindowHandle = mainWindowHandle;
        }
    }
}
