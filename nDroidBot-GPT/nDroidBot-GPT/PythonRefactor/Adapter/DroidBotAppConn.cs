using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

public class DroidBotAppConn
{
    private const string RemoteAddr = "tcp:7336";
    private const string PackageName = "io.github.ylimit.droidbotapp";
    private const int PacketHeadLen = 6;

    private TcpClient _client;
    private NetworkStream _stream;
    private Thread _listenThread;
    private readonly object _lock = new();
    private volatile bool _connected = false;
    private readonly ILogger _logger;
    private string _deviceSerial;
    private int _port;
    private string _host = "localhost";

    private JsonElement? _lastAccEvent;

    public DroidBotAppConn(string deviceSerial, int port, ILogger logger = null)
    {
        _deviceSerial = deviceSerial;
        _port = port;
        _logger = logger ?? new ConsoleLogger();
    }

    public void Connect()
    {
        try
        {
            ForwardPort(_port, RemoteAddr);

            _client = new TcpClient(_host, _port);
            _stream = _client.GetStream();

            _connected = true;

            _listenThread = new Thread(ListenMessages);
            _listenThread.Start();
        }
        catch (Exception ex)
        {
            _logger.Warn($"Failed to connect: {ex}");
            _connected = false;
            throw new DroidBotAppConnException();
        }
    }

    private void ForwardPort(int port, string remoteAddr)
    {
        string adbCmd = $"adb -s {_deviceSerial} forward tcp:{port} {remoteAddr}";
        var process = Process.Start(new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/C {adbCmd}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        });
        process.WaitForExit();
    }

    private void ListenMessages()
    {
        _logger.Debug("Start listening messages");
        try
        {
            while (_connected)
            {
                var header = SockRead(PacketHeadLen);
                var headerData = ParseHeader(header);
                int messageLen = headerData.messageLen;

                var messageBytes = SockRead(messageLen);
                var messageStr = Encoding.UTF8.GetString(messageBytes);

                HandleMessage(messageStr);
            }
        }
        catch (Exception ex)
        {
            _logger.Warn("Error in listener thread: " + ex);
            Disconnect();
        }
    }

    private byte[] SockRead(int length)
    {
        byte[] buffer = new byte[length];
        int bytesRead = 0;

        while (bytesRead < length)
        {
            int read = _stream.Read(buffer, bytesRead, length - bytesRead);
            if (read == 0)
                throw new EOFException();
            bytesRead += read;
        }

        return buffer;
    }

    private (byte, byte, int messageLen) ParseHeader(byte[] header)
    {
        byte a = header[0];
        byte b = header[1];
        int length = BitConverter.ToInt32(header[2..6].Reverse().ToArray()); // Big endian
        return (a, b, length);
    }

    private void HandleMessage(string message)
    {
        // TODO: Parse AccEvent and rotation messages
        _logger.Debug("Received: " + message);
    }

    public void Disconnect()
    {
        _connected = false;
        _stream?.Close();
        _client?.Close();

        // Remove port forwarding
        string removeCmd = $"adb -s {_deviceSerial} forward --remove tcp:{_port}";
        Process.Start("cmd.exe", $"/C {removeCmd}");
    }
}

public class DroidBotAppConnException : Exception
{
    public DroidBotAppConnException(string message = "DroidBot connection error") : base(message) { }
}

public class EOFException : Exception
{
    public EOFException(string message = "Unexpected EOF from socket") : base(message) { }
}

