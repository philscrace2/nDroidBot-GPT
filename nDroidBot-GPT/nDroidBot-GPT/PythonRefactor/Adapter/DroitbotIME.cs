using nDroidBot_GPT.PythonRefactor;
using nDroidBot_GPT.PythonRefactor.Adapter;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

public class DroidBotImeException : Exception
{
    public DroidBotImeException(string message = "DroidBot IME connection failed.") : base(message) { }
}

public class DroidBotIme : IAdapter
{
    private const string DroidBotAppPackage = "io.github.ylimit.droidbotapp";
    private const string ImeService = DroidBotAppPackage + "/.DroidBotIME";

    private readonly Device _device;
    private readonly ILogger _logger;
    private bool _connected;

    public DroidBotIme(Device device, ILogger logger = null)
    {
        _device = device ?? new Device();
        _logger = logger ?? new ConsoleLogger();
        _connected = false;
    }

    public void SetUp()
    {
        if (_device.Adb.GetInstalledApps().Contains(DroidBotAppPackage))
        {
            _logger.Debug("DroidBot app already installed.");
            return;
        }

        try
        {
            string apkPath = "resources/droidbotApp.apk"; // Adjust as needed
            _device.Adb.RunCmd($"install \"{apkPath}\"");
            _logger.Debug("DroidBot app installed.");
        }
        catch (Exception ex)
        {
            _logger.Warn(ex.Message);
            _logger.Warn("Failed to install DroidBot app.");
        }
    }

    public void TearDown()
    {
        _device.UninstallApp(DroidBotAppPackage);
    }

    public void Connect()
    {
        var result = _device.Adb.Shell($"ime enable {ImeService}");
        if (result.Contains("now enabled") || result.Contains("already enabled"))
        {
            var setResult = _device.Adb.Shell($"ime set {ImeService}");
            if (setResult.Contains($"{ImeService} selected"))
            {
                _connected = true;
                return;
            }
        }
        _logger.Warn("Failed to connect DroidBotIME!");
    }

    public bool CheckConnectivity() => _connected;

    public void Disconnect()
    {
        _connected = false;
        var result = _device.Adb.Shell($"ime disable {ImeService}");
        if (result.EndsWith("now disabled"))
        {
            Console.WriteLine($"[CONNECTION] {nameof(DroidBotIme)} is disconnected");
            return;
        }
        _logger.Warn("Failed to disconnect DroidBotIME!");
    }

    public void InputText(string text, int mode = 0)
    {
        string encodedText = text.Replace(" ", "--");
        string cmd = $"am broadcast -a DROIDBOT_INPUT_TEXT --es text \"{encodedText}\" --ei mode {mode}";
        _device.Adb.Shell(cmd);
    }
}
