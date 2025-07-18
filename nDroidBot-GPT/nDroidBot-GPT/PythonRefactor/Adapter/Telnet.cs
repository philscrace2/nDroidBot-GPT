using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class TelnetException : Exception
{
    public TelnetException(string message = "Telnet connection failed.") : base(message) { }
}

public class TelnetConsole
{
    private readonly object _lock = new object();
    private TcpClient _client;
    private StreamReader _reader;
    private StreamWriter _writer;
    private readonly string _authToken;
    private readonly string _deviceSerial;
    private readonly ILogger _logger;

    public TelnetConsole(string deviceSerial = null, string authToken = null, ILogger logger = null)
    {
        _deviceSerial = deviceSerial ?? "emulator-5554";
        _authToken = authToken;
        _logger = logger ?? new ConsoleLogger();
    }

    public void Connect()
    {
        if (!string.IsNullOrEmpty(_deviceSerial) && _deviceSerial.StartsWith("emulator-"))
        {
            string host = "localhost";
            int port = int.Parse(_deviceSerial.Substring(9));

            _client = new TcpClient();
            _client.Connect(host, port);
            NetworkStream stream = _client.GetStream();
            _reader = new StreamReader(stream, Encoding.ASCII);
            _writer = new StreamWriter(stream, Encoding.ASCII) { AutoFlush = true };

            if (!string.IsNullOrEmpty(_authToken))
            {
                RunCmd($"auth {_authToken}");
            }

            if (!CheckConnectivity())
            {
                throw new TelnetException("Connectivity check failed.");
            }

            _logger.Debug($"Telnet successfully initiated, port: {port}");
            return;
        }

        throw new TelnetException("Invalid device serial format.");
    }

    public string RunCmd(string command)
    {
        if (_client == null || !_client.Connected)
        {
            _logger.Warn("Telnet is not connected!");
            return null;
        }

        lock (_lock)
        {
            _logger.Debug($"command: {command}");

            _writer.WriteLine(command);

            var result = ReadUntil("OK", TimeSpan.FromSeconds(5));

            // Eat the rest
            ReadUntil("NEVER MATCH", TimeSpan.FromSeconds(1));

            _logger.Debug("return:");
            _logger.Debug(result);

            return result;
        }
    }

    private string ReadUntil(string match, TimeSpan timeout)
    {
        var sb = new StringBuilder();
        var buffer = new char[1];
        DateTime deadline = DateTime.Now + timeout;

        while (DateTime.Now < deadline)
        {
            if (_reader.Peek() >= 0)
            {
                _reader.Read(buffer, 0, 1);
                sb.Append(buffer[0]);
                if (sb.ToString().EndsWith(match))
                {
                    break;
                }
            }
            else
            {
                Thread.Sleep(10);
            }
        }

        return sb.ToString();
    }

    public bool CheckConnectivity()
    {
        try
        {
            RunCmd("help");
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Disconnect()
    {
        if (_client != null)
        {
            _writer?.Close();
            _reader?.Close();
            _client?.Close();
            _logger.Info($"[CONNECTION] {nameof(TelnetConsole)} is disconnected.");
        }
    }
}

public interface ILogger
{
    void Debug(string message);
    void Info(string message);
    void Warn(string message);
}

public class ConsoleLogger : ILogger
{
    public void Debug(string message) => Console.WriteLine("[DEBUG] " + message);
    public void Info(string message) => Console.WriteLine("[INFO] " + message);
    public void Warn(string message) => Console.WriteLine("[WARN] " + message);
}

