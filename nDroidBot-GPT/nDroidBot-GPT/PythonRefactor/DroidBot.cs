using System;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace nDroidBot_GPT.PythonRefactor
{
    public class DroidBot
    {
        private static DroidBot _instance;
        private readonly string _outputDir;
        private Timer _timer;
        private readonly bool _keepEnv;
        private readonly bool _keepApp;
        private readonly bool _cvMode;
        private readonly bool _debugMode;
        private readonly bool _enableAccessibilityHard;
        private readonly bool _humanoid;
        private readonly bool _ignoreAd;
        private readonly string _replayOutput;
        private readonly int? _eventCount;
        private readonly Device _device;
        private readonly App _app;
        private readonly AppEnvManager _envManager;
        private readonly InputManager _inputManager;

        private bool _enabled = true;
        private int _timeout;

        public static DroidBot Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException("DroidBot is not initiated!");
                }
                return _instance;
            }
        }

        public DroidBot(
            string apkPath = null,
            string deviceSerial = null,
            string task = null,
            bool isEmulator = false,
            string outputDir = null,
            string envPolicy = null,
            string policyName = null,
            bool randomInput = false,
            string scriptPath = null,
            int? eventCount = null,
            int? eventInterval = null,
            int? timeout = null,
            bool keepApp = false,
            bool keepEnv = false,
            bool cvMode = false,
            bool debugMode = false,
            string profilingMethod = null,
            bool grantPerm = false,
            bool enableAccessibilityHard = false,
            string master = null,
            bool humanoid = false,
            bool ignoreAd = false,
            string replayOutput = null)
        {
            _outputDir = outputDir;
            _timeout = timeout ?? 0;
            _keepEnv = keepEnv;
            _keepApp = keepApp;
            _cvMode = cvMode;
            _debugMode = debugMode;
            _enableAccessibilityHard = enableAccessibilityHard;
            _humanoid = humanoid;
            _ignoreAd = ignoreAd;
            _replayOutput = replayOutput;
            _eventCount = eventCount;

            if (_outputDir != null)
            {
                if (!Directory.Exists(_outputDir))
                {
                    Directory.CreateDirectory(_outputDir);
                }

                string htmlIndexPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", "index.html");
                string stylesheetsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", "stylesheets");
                string targetStylesheetsDir = Path.Combine(_outputDir, "stylesheets");

                if (Directory.Exists(targetStylesheetsDir))
                {
                    Directory.Delete(targetStylesheetsDir, true);
                }

                File.Copy(htmlIndexPath, Path.Combine(_outputDir, "index.html"));
                CopyDirectory(stylesheetsPath, targetStylesheetsDir);
            }

            _instance = this;

            try
            {
                _device = new Device(deviceSerial, isEmulator, _outputDir, _cvMode, grantPerm, null, _enableAccessibilityHard, _humanoid, _ignoreAd);
                _app = new App(apkPath, _outputDir);
                _envManager = new AppEnvManager(_device, _app, envPolicy);
                _inputManager = new InputManager(_device, _app, task, policyName, randomInput, eventCount, eventInterval, scriptPath, profilingMethod, master, replayOutput);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Stop();
                Environment.Exit(-1);
            }
        }

        public void Start()
        {
            if (!_enabled) return;
            Console.WriteLine("Starting DroidBot");

            try
            {
                if (_timeout > 0)
                {
                    _timer = new Timer(_ => Stop(), null, _timeout * 1000, Timeout.Infinite);
                }

                _device.SetUp();
                if (!_enabled) return;
                _device.Connect();

                if (!_enabled) return;
                _device.InstallApp(_app);

                if (!_enabled) return;
                _envManager.Deploy();

                if (!_enabled) return;

                _inputManager.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Stop();
                Environment.Exit(-1);
            }

            Stop();
            Console.WriteLine("DroidBot Stopped");
        }

        public void Stop()
        {
            _enabled = false;

            if (_timer != null && _timer.Change(Timeout.Infinite, Timeout.Infinite))
            {
                _timer.Dispose();
            }

            _envManager?.Stop();
            _inputManager?.Stop();
            _device?.Disconnect();

            if (!_keepEnv)
            {
                _device.TearDown();
            }

            if (!_keepApp)
            {
                _device.UninstallApp(_app);
            }
        }

        private void CopyDirectory(string sourceDir, string destDir)
        {
            Directory.CreateDirectory(destDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                File.Copy(file, Path.Combine(destDir, Path.GetFileName(file)));
            }

            foreach (var subDir in Directory.GetDirectories(sourceDir))
            {
                string destSubDir = Path.Combine(destDir, Path.GetFileName(subDir));
                CopyDirectory(subDir, destSubDir);
            }
        }
    }

    public class DroidBotException : Exception
    {
        public DroidBotException(string message) : base(message) { }
    }
}
