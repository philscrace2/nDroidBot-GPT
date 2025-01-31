using System;
using System.Diagnostics;

namespace nDroidBot_GPT
{
    public class Adapter
    {
        public string ExecuteAdbCommand(string command)
        {
            try
            {
                ProcessStartInfo processInfo = new ProcessStartInfo("adb", command)
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                Process process = Process.Start(processInfo);
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return output;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing ADB command: {ex.Message}");
                return string.Empty;
            }
        }

        public string DumpViews()
        {
            return ExecuteAdbCommand("shell uiautomator dump");
        }
    }

}
