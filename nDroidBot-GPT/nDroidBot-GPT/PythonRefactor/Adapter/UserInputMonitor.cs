using nDroidBot_GPT.PythonRefactor;
using nDroidBot_GPT.PythonRefactor.Adapter;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

public class UserInputMonitor : IAdapter
{
    private readonly Device _device;
    private readonly ILogger _logger;
    private Process _process;
    private bool _connected;
    private string _outFile;

    public UserInputMonitor(Device device, ILogger logger = null)
    {
        _device = device ?? new Device();
        _logger = logger ?? new ConsoleLogger();

        if (!string.IsNullOrWhiteSpace(_device.OutputDir))
        {
            _outFile = Path.Combine(_device.OutputDir, "user_input.txt");
        }
    }

    public void Connect()
    {
        var psi = new ProcessStartInfo
        {
            FileName = "adb",
            Arguments = $"-s {_device.Serial} shell getevent -lt",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        _process = new Process { StartInfo = psi };
        _process.Start();

        var listenerThread = new Thread(HandleOutput);
        listenerThread.Start();
    }

    public void Disconnect()
    {
        _connected = false;
        try
        {
            _process?.Kill(true);
        }
        catch (Exception ex)
        {
            _logger.Warn("Failed to terminate getevent process: " + ex.Message);
        }
    }

    public bool CheckConnectivity() => _connected;

    private void HandleOutput()
    {
        _connected = true;

        StreamWriter fileWriter = null;
        if (_outFile != null)
        {
            fileWriter = new StreamWriter(_outFile, false, Encoding.UTF8);
        }

        try
        {
            while (_connected && !_process.HasExited)
            {
                var line = _process.StandardOutput.ReadLine();
                if (line == null) continue;

                ParseLine(line);
                fileWriter?.WriteLine(line);
            }
        }
        catch (Exception ex)
        {
            _logger.Warn("Error reading getevent output: " + ex.Message);
        }
        finally
        {
            fileWriter?.Close();
            Console.WriteLine($"[CONNECTION] {nameof(UserInputMonitor)} is disconnected");
        }
    }

    private void ParseLine(string line)
    {
        // Optional: implement parsing logic for getevent data if needed
        _logger.Debug("[GETEVENT] " + line.Trim());
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
