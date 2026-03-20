using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows.Automation;

namespace nTestar.UiaProbe;

internal static class Program
{
    private const int MaxDepth = 4;
    private const int MaxNodes = 2000;
    private const int StartupTimeoutMs = 15000;

    [STAThread]
    private static int Main()
    {
        if (!OperatingSystem.IsWindows())
        {
            Console.WriteLine("This probe must run on Windows.");
            return 1;
        }

        Process? notepad = null;
        try
        {
            HashSet<int> existingNotepadPids = GetNotepadProcessIds();
            notepad = StartNotepad();
            if (notepad == null)
            {
                Console.WriteLine("Failed to start notepad.exe");
                return 1;
            }

            (Process? resolvedProcess, IntPtr hwnd) = WaitForNotepadWindow(notepad, existingNotepadPids, StartupTimeoutMs);
            if (resolvedProcess != null)
            {
                notepad = resolvedProcess;
            }

            if (hwnd == IntPtr.Zero)
            {
                Console.WriteLine($"Notepad started (pid={notepad.Id}) but no main window was found.");
                return 1;
            }

            AutomationElement windowElement = AutomationElement.FromHandle(hwnd);
            if (windowElement == null)
            {
                Console.WriteLine($"Failed to create AutomationElement from hwnd=0x{hwnd.ToInt64():X}.");
                return 1;
            }

            var snapshot = BuildSnapshot(notepad, hwnd, windowElement);
            string json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions { WriteIndented = true });
            string outputPath = Path.Combine(AppContext.BaseDirectory, "uia-probe-notepad-only.json");
            File.WriteAllText(outputPath, json);

            Console.WriteLine($"Notepad PID: {snapshot.NotepadProcessId}");
            Console.WriteLine($"Main HWND: 0x{snapshot.MainWindowHandle:X}");
            Console.WriteLine($"Captured nodes: {snapshot.CapturedNodeCount}");
            Console.WriteLine($"Only notepad process IDs observed: {snapshot.OnlyNotepadProcessIds}");
            Console.WriteLine($"Observed process IDs: {string.Join(", ", snapshot.ObservedProcessIds)}");
            Console.WriteLine($"Wrote: {outputPath}");

            if (!snapshot.OnlyNotepadProcessIds)
            {
                Console.WriteLine("Probe detected UIA nodes outside notepad.exe process.");
                return 2;
            }

            Console.WriteLine("Probe confirmed: captured UIA tree is restricted to notepad.exe.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Probe failed: {ex}");
            return 1;
        }
        finally
        {
            // Keep Notepad open so you can visually confirm if needed.
            Console.Write("Press Enter to close probe");
            _ = Console.ReadLine();

            if (notepad != null && !notepad.HasExited)
            {
                try
                {
                    notepad.Kill(entireProcessTree: true);
                    notepad.WaitForExit(2000);
                }
                catch
                {
                    // Best effort cleanup.
                }
            }
        }
    }

    private static Process? StartNotepad()
    {
        string notepadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "notepad.exe");
        return Process.Start(new ProcessStartInfo
        {
            FileName = notepadPath,
            UseShellExecute = false
        });
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

    private static ProbeSnapshot BuildSnapshot(Process notepad, IntPtr hwnd, AutomationElement windowElement)
    {
        var observedPids = new HashSet<int>();
        int capturedCount = 0;
        UiaNode? root = BuildNode(windowElement, notepad.Id, 0, ref capturedCount, observedPids);

        return new ProbeSnapshot
        {
            CapturedAtUtc = DateTime.UtcNow,
            NotepadProcessId = notepad.Id,
            MainWindowHandle = hwnd.ToInt64(),
            CapturedNodeCount = capturedCount,
            ObservedProcessIds = observedPids.OrderBy(p => p).ToList(),
            OnlyNotepadProcessIds = observedPids.All(pid => pid == notepad.Id),
            Root = root
        };
    }

    private static UiaNode? BuildNode(
        AutomationElement element,
        int expectedPid,
        int depth,
        ref int capturedCount,
        HashSet<int> observedPids)
    {
        if (capturedCount >= MaxNodes || depth > MaxDepth)
        {
            return null;
        }

        AutomationElement.AutomationElementInformation info;
        try
        {
            info = element.Current;
        }
        catch
        {
            return null;
        }

        observedPids.Add(info.ProcessId);
        capturedCount++;

        var node = new UiaNode
        {
            Name = info.Name ?? string.Empty,
            ControlType = info.ControlType?.ProgrammaticName ?? string.Empty,
            ClassName = info.ClassName ?? string.Empty,
            AutomationId = info.AutomationId ?? string.Empty,
            FrameworkId = info.FrameworkId ?? string.Empty,
            ProcessId = info.ProcessId,
            NativeWindowHandle = info.NativeWindowHandle,
            IsOffscreen = info.IsOffscreen,
            IsEnabled = info.IsEnabled,
            IsExpectedProcess = info.ProcessId == expectedPid,
            BoundingRectangle = new RectSnapshot
            {
                X = SafeDouble(info.BoundingRectangle.X),
                Y = SafeDouble(info.BoundingRectangle.Y),
                Width = SafeDouble(info.BoundingRectangle.Width),
                Height = SafeDouble(info.BoundingRectangle.Height)
            }
        };

        if (depth >= MaxDepth || capturedCount >= MaxNodes)
        {
            return node;
        }

        AutomationElementCollection children;
        try
        {
            children = element.FindAll(TreeScope.Children, Condition.TrueCondition);
        }
        catch
        {
            return node;
        }

        for (int i = 0; i < children.Count && capturedCount < MaxNodes; i++)
        {
            AutomationElement child = children[i];
            UiaNode? childNode = BuildNode(child, expectedPid, depth + 1, ref capturedCount, observedPids);
            if (childNode != null)
            {
                node.Children.Add(childNode);
            }
        }

        return node;
    }

    private static double SafeDouble(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            return 0;
        }

        return value;
    }

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

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

internal sealed class ProbeSnapshot
{
    public DateTime CapturedAtUtc { get; set; }
    public int NotepadProcessId { get; set; }
    public long MainWindowHandle { get; set; }
    public int CapturedNodeCount { get; set; }
    public List<int> ObservedProcessIds { get; set; } = new();
    public bool OnlyNotepadProcessIds { get; set; }
    public UiaNode? Root { get; set; }
}

internal sealed class UiaNode
{
    public string Name { get; set; } = string.Empty;
    public string ControlType { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string AutomationId { get; set; } = string.Empty;
    public string FrameworkId { get; set; } = string.Empty;
    public int ProcessId { get; set; }
    public bool IsExpectedProcess { get; set; }
    public int NativeWindowHandle { get; set; }
    public bool IsOffscreen { get; set; }
    public bool IsEnabled { get; set; }
    public RectSnapshot? BoundingRectangle { get; set; }
    public List<UiaNode> Children { get; set; } = new();
}

internal sealed class RectSnapshot
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
}
