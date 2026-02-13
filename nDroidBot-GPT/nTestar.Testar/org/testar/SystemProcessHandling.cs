namespace org.testar
{
    public static class SystemProcessHandling
    {
        public static List<ProcessInfo> getRunningProcesses(string marker)
        {
            var processes = new List<ProcessInfo>();
            foreach (var process in System.Diagnostics.Process.GetProcesses())
            {
                try
                {
                    processes.Add(new ProcessInfo
                    {
                        Pid = process.Id,
                        Name = process.ProcessName
                    });
                }
                catch
                {
                    // Ignore inaccessible processes.
                }
            }

            return processes;
        }
    }
}
