using nDroidBot_GPT.PythonRefactor;
using nDroidBot_GPT.PythonRefactor.Adapter;
using System.Diagnostics;
using System.Text;

public class Logcat : IAdapter
{
    private readonly Device _device;
    private readonly ILogger _logger;
    private readonly List<ILogcatParser> _parsers = new();
    private readonly List<string> _recentLines = new();

    private Process _process;
    private bool _connected;
    private string _outFile;

    public Logcat(Device device, ILogger logger = null)
    {
        _device = device ?? new Device();
        _logger = logger ?? new ConsoleLogger();
        _connected = false;

        if (!string.IsNullOrWhiteSpace(_device.OutputDir))
        {
            _outFile = Path.Combine(_device.OutputDir, "logcat.txt");
        }
    }

    public void Connect()
    {
        _device.Adb.RunCmd("logcat -c");

        var psi = new ProcessStartInfo
        {
            FileName = "adb",
            Arguments = $"-s {_device.Serial} logcat -v threadtime *:I",
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
            _logger.Warn("Failed to terminate logcat process: " + ex.Message);
        }
    }

    public bool CheckConnectivity() => _connected;

    public List<string> GetRecentLines()
    {
        lock (_recentLines)
        {
            var lines = new List<string>(_recentLines);
            _recentLines.Clear();
            return lines;
        }
    }

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

                lock (_recentLines)
                {
                    _recentLines.Add(line);
                }

                ParseLine(line);
                fileWriter?.WriteLine(line);
            }
        }
        catch (Exception ex)
        {
            _logger.Warn("Error reading logcat output: " + ex.Message);
        }
        finally
        {
            fileWriter?.Close();
            Console.WriteLine($"[CONNECTION] {nameof(Logcat)} is disconnected");
        }
    }

    private void ParseLine(string line)
    {
        foreach (var parser in _parsers)
        {
            parser.Parse(line);
        }
    }

    public void AddParser(ILogcatParser parser) => _parsers.Add(parser);

    public void SetUp()
    {
        throw new NotImplementedException();
    }

    public void TearDown()
    {
        throw new NotImplementedException();
    }
}

public interface ILogcatParser
{
    void Parse(string logcatLine);
}
