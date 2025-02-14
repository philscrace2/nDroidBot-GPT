using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using nDroidBot_GPT;

namespace nDroidBot_GPT.PythonRefactor
{
    public class Device
    {
        private static readonly string DefaultNumber = "1234567890";
        private static readonly string DefaultContent = "Hello world!";
        private readonly string serial;
        private readonly bool isEmulator;
        private readonly bool cvMode;
        private readonly string outputDir;
        private readonly bool grantPerm;
        private readonly bool enableAccessibilityHard;
        private readonly string humanoid;
        private readonly bool ignoreAd;
        private readonly Dictionary<IAdapter, bool> adapters = new Dictionary<IAdapter, bool>();
        private readonly ADB adb;
        private readonly TelnetConsole telnet;
        private readonly DroidBotAppConn droidbotApp;
        private readonly Minicap minicap;
        private readonly Logcat logcat;
        private readonly UserInputMonitor userInputMonitor;
        private readonly ProcessMonitor processMonitor;
        private readonly DroidBotIme droidBotIme;

        private bool connected;
        private object lastKnownState;
        private List<int> usedPorts = new List<int>();
        private bool pauseSendingEvent;

        public Device(string deviceSerial = null, bool isEmulator = false, string outputDir = null,
                      bool cvMode = false, bool grantPerm = false, string telnetAuthToken = null,
                      bool enableAccessibilityHard = false, string humanoid = null, bool ignoreAd = false)
        {
            this.serial = deviceSerial ?? throw new ArgumentNullException(nameof(deviceSerial));
            this.isEmulator = isEmulator;
            this.cvMode = cvMode;
            this.outputDir = outputDir;
            this.grantPerm = grantPerm;
            this.enableAccessibilityHard = enableAccessibilityHard;
            this.humanoid = humanoid;
            this.ignoreAd = ignoreAd;

            // Initialize the adapters
            this.adb = new ADB(this);
            this.telnet = new TelnetConsole(this, telnetAuthToken);
            this.droidbotApp = new DroidBotAppConn(this);
            this.minicap = new Minicap(this);
            this.logcat = new Logcat(this);
            this.userInputMonitor = new UserInputMonitor(this);
            this.processMonitor = new ProcessMonitor(this);
            this.droidBotIme = new DroidBotIme(this);

            // Add adapters to the dictionary
            adapters.Add(adb, true);
            adapters.Add(telnet, false);
            adapters.Add(droidbotApp, true);
            adapters.Add(minicap, true);
            adapters.Add(logcat, true);
            adapters.Add(userInputMonitor, true);
            adapters.Add(processMonitor, true);
            adapters.Add(droidBotIme, true);

            if (isEmulator)
            {
                Console.WriteLine("Disabling Minicap for emulator.");
                adapters[minicap] = false;
            }
        }

        public void CheckConnectivity()
        {
            foreach (var adapter in adapters)
            {
                var adapterName = adapter.Key.GetType().Name;
                var adapterEnabled = adapter.Value;

                if (!adapterEnabled)
                {
                    Console.WriteLine($"[CONNECTION] {adapterName} is not enabled.");
                }
                else
                {
                    if (adapter.Key.CheckConnectivity())
                    {
                        Console.WriteLine($"[CONNECTION] {adapterName} is enabled and connected.");
                    }
                    else
                    {
                        Console.WriteLine($"[CONNECTION] {adapterName} is enabled but not connected.");
                    }
                }
            }
        }

        public void WaitForDevice()
        {
            Console.WriteLine("Waiting for device...");
            try
            {
                Process.Start("adb", $"-s {serial} wait-for-device").WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error waiting for device: {ex.Message}");
            }
        }

        public void SetUp()
        {
            // Wait for emulator to start
            WaitForDevice();
            foreach (var adapter in adapters)
            {
                if (adapter.Value)
                {
                    adapter.Key.SetUp();
                }
            }
        }

        public void Connect()
        {
            foreach (var adapter in adapters)
            {
                if (adapter.Value)
                {
                    adapter.Key.Connect();
                }
            }

            // Retrieve device info
            GetSdkVersion();
            GetReleaseVersion();
            GetRoSecure();
            GetRoDebuggable();
            GetDisplayInfo();
            Unlock();
            CheckConnectivity();
            connected = true;
        }

        public void Disconnect()
        {
            connected = false;
            foreach (var adapter in adapters)
            {
                if (adapter.Value)
                {
                    adapter.Key.Disconnect();
                }
            }

            if (outputDir != null)
            {
                var tempDir = Path.Combine(outputDir, "temp");
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        public void TearDown()
        {
            foreach (var adapter in adapters)
            {
                if (adapter.Value)
                {
                    adapter.Key.TearDown();
                }
            }
        }

        public bool IsForeground(App app)
        {
            var packageName = app?.GetPackageName();
internal void UninstallApp(App app)
        {
            throw new NotImplementedException();
        }
    }

        internal void UninstallApp(App app)
        {
            throw new NotImplementedException();
        }

        internal void InstallApp(App app)
        {
            throw new NotImplementedException();
        }
    }



