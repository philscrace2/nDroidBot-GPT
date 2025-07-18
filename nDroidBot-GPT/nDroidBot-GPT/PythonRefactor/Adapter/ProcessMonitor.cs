using nDroidBot_GPT.PythonRefactor;
using nDroidBot_GPT.PythonRefactor.Adapter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class ProcessMonitor : IAdapter
{
    private readonly Device _device;
    private readonly App _app;
    private readonly ILogger _logger;

    private readonly Dictionary<string, string> _pid2user = new();
    private readonly Dictionary<string, string> _pid2ppid = new();
    private readonly Dictionary<string, string> _pid2name = new();
    private readonly HashSet<IProcessStateListener> _listeners = new();

    private readonly object _lock = new();
    private bool _enabled;

    public ProcessMonitor(Device device, App app = null, ILogger logger = null)
    {
        _device = device;
        _app = app;
        _logger = logger ?? new ConsoleLogger();
    }

    public void AddStateListener(IProcessStateListener listener)
    {
        lock (_listeners)
        {
            _listeners.Add(listener);
        }
    }

    public void RemoveStateListener(IProcessStateListener listener)
    {
        lock (_listeners)
        {
            _listeners.Remove(listener);
        }
    }

    public bool Connect()
    {
        _enabled = true;
        var thread = new Thread(MaintainProcessMapping);
        thread.Start();
        return true;
    }

    public void Disconnect()
    {
        _enabled = false;
    }

    public bool CheckConnectivity() => _enabled;

    private void MaintainProcessMapping()
    {
        while (_enabled)
        {
            var psCmd = _device != null
                ? $"adb -s {_device.Serial} shell ps"
                : "adb shell ps";

            try
            {
                var output = RunCommand(psCmd);
                if (string.IsNullOrWhiteSpace(output)) continue;

                var lines = output.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length < 2) continue;

                var header = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (!(header[0] == "USER" && header[1] == "PID" && header[2] == "PPID" && header[^1] == "NAME"))
                {
                    _logger.Warn("Unexpected ps output format: " + string.Join(", ", header));
                    continue;
                }

                lock (_lock)
                {
                    _pid2name.Clear();
                    _pid2ppid.Clear();
                    _pid2user.Clear();

                    foreach (var line in lines.Skip(1))
                    {
                        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 4) continue;

                        string user = parts[0];
                        string pid = parts[1];
                        string ppid = parts[2];
                        string name = parts[^1];

                        _pid2name[pid] = name;
                        _pid2ppid[pid] = ppid;
                        _pid2user[pid] = user;
                    }
                }

                NotifyListeners();

            }
            catch (Exception ex)
            {
                _logger.Warn("Error reading ps output: " + ex.Message);
            }

            Thread.Sleep(1000);
        }

        Console.WriteLine($"[CONNECTION] {nameof(ProcessMonitor)} is disconnected");
    }

    private void NotifyListeners()
    {
        // Optional: notify listeners with current process snapshot
        lock (_listeners)
        {
            foreach (var listener in _listeners)
            {
                listener.OnStateUpdated(_pid2name);
            }
        }
    }

    public List<string> GetPpidsByPid(string pid)
    {
        var ppids = new List<string>();
        lock (_lock)
        {
            while (_pid2ppid.ContainsKey(pid))
            {
                ppids.Add(pid);
                pid = _pid2ppid[pid];
            }
        }
        ppids.Reverse();
        return ppids;
    }

    public List<string> GetNamesByPid(string pid)
    {
        var ppids = GetPpidsByPid(pid);
        var names = new List<string>();
        lock (_lock)
        {
            foreach (var ppid in ppids)
            {
                if (_pid2name.TryGetValue(ppid, out var name))
                {
                    names.Add(name);
                }
            }
        }
        return names;
    }

    private string RunCommand(string cmd)
    {
        var psi = new ProcessStartInfo("cmd.exe", $"/C {cmd}")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = psi };
        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return output;
    }

    void IAdapter.Connect()
    {
        throw new NotImplementedException();
    }

    public void SetUp()
    {
        throw new NotImplementedException();
    }

    public void TearDown()
    {
        throw new NotImplementedException();
    }
}

