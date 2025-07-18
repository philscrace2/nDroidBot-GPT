using nDroidBot_GPT.PythonRefactor;
using nDroidBot_GPT.PythonRefactor.Adapter;
using System;
using System.Diagnostics;
using System.Net.Sockets;

public class MinicapException : Exception
{
    public MinicapException(string message = "Minicap connection error") : base(message) { }
}

public class Minicap : IAdapter
{
    private readonly ILogger _logger;
    private readonly string _host = "localhost";
    private readonly int _port;
    private readonly Device _device;

    private TcpClient _client;
    private NetworkStream _stream;
    private Thread _listenerThread;
    private Process _minicapProcess;

    private byte[] _lastScreen;
    private DateTime? _lastScreenTime;
    private bool _connected = false;
    private int _width = -1, _height = -1, _orientation = -1;

    private const int RotationCheckIntervalSeconds = 1;
    private DateTime _lastRotationCheck = DateTime.MinValue;
    private List<ViewNode> _lastViews;

    public Minicap(Device device, ILogger logger = null)
    {
        _device = device;
        _port = device.GetRandomPort();
        _logger = logger ?? new ConsoleLogger();
    }

    public void SetUp()
    {
        try
        {
            var minicapFiles = _device.Adb.Shell($"ls /data/local/tmp/minicap-devel 2>/dev/null").Split();
            if (minicapFiles.Contains("minicap.so") && (minicapFiles.Contains("minicap") || minicapFiles.Contains("minicap-nopie")))
            {
                _logger.Debug("Minicap already installed.");
                return;
            }
        }
        catch { }

        try
        {
            _device.Adb.Shell("mkdir /data/local/tmp/minicap-devel");
        }
        catch { }

        var abi = _device.Adb.GetProp("ro.product.cpu.abi");
        var sdk = _device.GetSdkVersion();
        var binName = sdk >= 16 ? "minicap" : "minicap-nopie";

        // For now assume the local paths are known (can load from embedded resource later)
        var localPath = "resources/minicap";
        var binPath = Path.Combine(localPath, "libs", abi, binName);
        var soPath = Path.Combine(localPath, "jni", "libs", $"android-{sdk}", abi, "minicap.so");

        _device.PushFile(binPath, "/data/local/tmp/minicap-devel");
        _device.PushFile(soPath, "/data/local/tmp/minicap-devel");
        _logger.Debug("Minicap installed.");
    }

    public void Connect()
    {
        var display = _device.GetDisplayInfo(refresh: true);
        if (!display.TryGetValue("width", out var widthObj) ||
            !display.TryGetValue("height", out var heightObj) ||
            !display.TryGetValue("orientation", out var orientationObj))
        {
            _logger.Warn("Cannot get display info.");
            return;
        }

        int w = Convert.ToInt32(widthObj);
        int h = Convert.ToInt32(heightObj);
        if (w > h) (w, h) = (h, w);
        int o = Convert.ToInt32(orientationObj) * 90;

        _width = w;
        _height = h;
        _orientation = o;

        string sizeOpt = $"{w}x{h}@{w}x{h}/{o}";

        string chmodCmd = $"adb -s {_device.Serial} shell chmod -R a+x /data/local/tmp/minicap-devel";
        string startCmd = $"adb -s {_device.Serial} shell LD_LIBRARY_PATH=/data/local/tmp/minicap-devel /data/local/tmp/minicap-devel/minicap -P {sizeOpt}";

        Process.Start("cmd.exe", $"/C {chmodCmd}").WaitForExit();
        _minicapProcess = Process.Start(new ProcessStartInfo("cmd.exe", $"/C {startCmd}")
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false
        });

        Thread.Sleep(2000);

        string forwardCmd = $"adb -s {_device.Serial} forward tcp:{_port} localabstract:minicap";
        Process.Start("cmd.exe", $"/C {forwardCmd}").WaitForExit();

        _client = new TcpClient(_host, _port);
        _stream = _client.GetStream();

        _connected = true;
        _listenerThread = new Thread(ListenMessages);
        _listenerThread.Start();

        _logger.Debug("Minicap connected.");
    }

    public bool CheckConnectivity()
    {
        if (!_connected)
            return false;

        if (_lastScreenTime == null)
            return false;

        return true;
    }

    public void Disconnect()
    {
        _connected = false;

        if (_client != null)
        {
            try
            {
                _stream?.Close();
                _client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        if (_minicapProcess != null)
        {
            try
            {
                _minicapProcess.Kill(true); // .NET 8 Kill method with child processes
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        try
        {
            string removeForwardCmd = $"adb -s {_device.Serial} forward --remove tcp:{_port}";
            var process = Process.Start(new ProcessStartInfo("cmd.exe", $"/C {removeForwardCmd}")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            });
            process.WaitForExit();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    public class ViewNode
    {
        public string Class { get; set; }
        public int[][] Bounds { get; set; }
        public bool Enabled { get; set; }
        public int TempId { get; set; }
        public string Signature { get; set; } = "";
        public int Parent { get; set; }
        public List<int> Children { get; set; } = new();
    }


    public List<ViewNode> GetViews()
    {
        if (_lastScreen == null)
        {
            _logger.Warn("last_screen is None");
            return null;
        }

        if (_lastViews != null)
            return _lastViews;

        using var img = Cv2.ImDecode(_lastScreen, ImreadModes.Color);
        if (img.Empty())
        {
            _logger.Warn("Failed to decode screen image.");
            return null;
        }

        var viewBounds = FindViews(img); // implements basic blob/contour detection

        var views = new List<ViewNode>();
        var root = new ViewNode
        {
            Class = "CVViewRoot",
            Bounds = new[] { new[] { 0, 0 }, new[] { _width, _height } },
            Enabled = true,
            TempId = 0
        };
        views.Add(root);

        int tempId = 1;
        foreach (var rect in viewBounds)
        {
            var (x, y, w, h) = rect;
            var roi = new Mat(img, new Rect(x, y, w, h));
            var hash = CalculateDHash(roi);

            var view = new ViewNode
            {
                Class = "CVView",
                Bounds = new[] { new[] { x, y }, new[] { x + w, y + h } },
                Enabled = true,
                TempId = tempId,
                Signature = hash,
                Parent = 0
            };
            views.Add(view);
            tempId++;
        }

        root.Children = Enumerable.Range(1, tempId - 1).ToList();
        _lastViews = views;

        return views;
    }


    public void TearDown()
    {
        throw new NotImplementedException();
    }
}


