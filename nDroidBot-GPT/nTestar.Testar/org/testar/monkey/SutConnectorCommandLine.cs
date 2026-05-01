using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using org.testar.monkey.alayer;
using org.testar.monkey.alayer.devices;
using org.testar.monkey.alayer.exceptions;
using org.testar.settings;

namespace org.testar.monkey
{
    public class SutConnectorCommandLine
    {
        private readonly StateBuilder? builder;
        private readonly bool processListenerOracleEnabled;
        private readonly Settings settings;

        public SutConnectorCommandLine(StateBuilder? builder, bool processListenerOracleEnabled, Settings settings)
        {
            this.builder = builder;
            this.processListenerOracleEnabled = processListenerOracleEnabled;
            this.settings = settings;
        }

        public SUT startOrConnectSut()
        {
            string commandLine = settings.get(ConfigTags.SUTConnectorValue, string.Empty).Trim();
            Process? process = StartProcess(commandLine);

            HashSet<int> existingNotepadPids = GetNotepadProcessIds();

            ////(Process? resolvedProcess, IntPtr hwnd) = WaitForNotepadWindow(process, existingNotepadPids,15000);

            if (process == null)
            {
                throw new SystemStartException($"Unable to start SUT from command line: {commandLine}");
            }

            Util.pause(settings.get(ConfigTags.StartupTime, 0.0));
            ConnectedSut connectedSut = new ConnectedSut(process, new AWTMouse(), new AWTKeyboard());
            return connectedSut;
        }

        private static Process? StartProcess(string commandLine)
        {
            IReadOnlyList<string> parts = SplitCommandLine(commandLine);
            if (parts.Count == 0)
            {
                return null;
            }

            var psi = new ProcessStartInfo
            {
                FileName = parts[0],
                UseShellExecute = false
            };

            for (int i = 1; i < parts.Count; i++)
            {
                psi.ArgumentList.Add(parts[i]);
            }

            try
            {
                return Process.Start(psi);
            }
            catch
            {
                return null;
            }
        }

        private static IReadOnlyList<string> SplitCommandLine(string commandLine)
        {
            var tokens = new List<string>();
            var current = new StringBuilder();
            bool inQuotes = false;

            foreach (char c in commandLine)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }

                if (!inQuotes && char.IsWhiteSpace(c))
                {
                    if (current.Length > 0)
                    {
                        tokens.Add(current.ToString());
                        current.Clear();
                    }

                    continue;
                }

                current.Append(c);
            }

            if (current.Length > 0)
            {
                tokens.Add(current.ToString());
            }

            return tokens;
        }

        private static (Process? Process, IntPtr Hwnd) WaitForNotepadWindow(
        Process launchProcess,
        HashSet<int> existingNotepadPids,
        int timeoutMs)
        {
            try
            {
                _ = launchProcess.WaitForInputIdle(2000);
            }
            catch
            {
                // Ignore: this can throw for some process models.
            }

            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                if (!launchProcess.HasExited)
                {
                    launchProcess.Refresh();
                    if (launchProcess.MainWindowHandle != IntPtr.Zero)
                    {
                        return (launchProcess, launchProcess.MainWindowHandle);
                    }

                    IntPtr launchedPidWindow = FindTopLevelWindowByPid(launchProcess.Id);
                    if (launchedPidWindow != IntPtr.Zero)
                    {
                        return (launchProcess, launchedPidWindow);
                    }
                }

                IntPtr anyNewNotepadWindow = FindAnyNewNotepadWindow(existingNotepadPids);
                if (anyNewNotepadWindow != IntPtr.Zero)
                {
                    int pid = GetWindowProcessId(anyNewNotepadWindow);
                    if (pid > 0)
                    {
                        try
                        {
                            return (Process.GetProcessById(pid), anyNewNotepadWindow);
                        }
                        catch
                        {
                            return (launchProcess, anyNewNotepadWindow);
                        }
                    }
                }

                Thread.Sleep(100);
            }

            return (launchProcess, IntPtr.Zero);
        }

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern short VkKeyScan(char ch);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        private static IntPtr FindTopLevelWindowByPid(int pid)
        {
            IntPtr found = IntPtr.Zero;
            EnumWindows((hWnd, _) =>
            {
                if (hWnd == IntPtr.Zero || !IsWindowVisible(hWnd))
                {
                    return true;
                }

                GetWindowThreadProcessId(hWnd, out uint windowPid);
                if (windowPid == (uint)pid)
                {
                    found = hWnd;
                    return false;
                }

                return true;
            }, IntPtr.Zero);

            return found;
        }

        private static IntPtr FindAnyNewNotepadWindow(HashSet<int> existingNotepadPids)
        {
            IntPtr found = IntPtr.Zero;
            EnumWindows((hWnd, _) =>
            {
                if (hWnd == IntPtr.Zero || !IsWindowVisible(hWnd))
                {
                    return true;
                }

                int pid = GetWindowProcessId(hWnd);
                if (pid <= 0 || existingNotepadPids.Contains(pid))
                {
                    return true;
                }

                try
                {
                    Process process = Process.GetProcessById(pid);
                    if (string.Equals(process.ProcessName, "notepad", StringComparison.OrdinalIgnoreCase))
                    {
                        found = hWnd;
                        return false;
                    }
                }
                catch
                {
                    // Ignore inaccessible or exited process.
                }

                return true;
            }, IntPtr.Zero);

            return found;
        }

        private static int GetWindowProcessId(IntPtr hWnd)
        {
            GetWindowThreadProcessId(hWnd, out uint processId);
            return unchecked((int)processId);
        }

        private static HashSet<int> GetNotepadProcessIds()
        {
            return Process.GetProcessesByName("notepad")
                .Select(p =>
                {
                    try
                    {
                        return p.Id;
                    }
                    catch
                    {
                        return 0;
                    }
                })
                .Where(id => id > 0)
                .ToHashSet();
        }
    }
}
