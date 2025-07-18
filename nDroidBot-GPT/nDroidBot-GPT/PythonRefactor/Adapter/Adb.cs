using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace nDroidBot_GPT.PythonRefactor.Adapter
{
    public class ADB : IAdapter
    {
        private static readonly string MODEL_PROPERTY = "ro.product.model";
        private static readonly string VERSION_SDK_PROPERTY = "ro.build.version.sdk";
        private static readonly string VERSION_RELEASE_PROPERTY = "ro.build.version.release";
        private static readonly string RO_SECURE_PROPERTY = "ro.secure";
        private static readonly string RO_DEBUGGABLE_PROPERTY = "ro.debuggable";

        private string serial;
        private string[] cmdPrefix;
        private bool attached;
        private string logPath;
        private int pid;

        public ADB(string deviceSerial)
        {
            this.serial = deviceSerial;
            this.cmdPrefix = new string[] { "adb", "-s", serial };
        }

        private string RunCmd(string[] extraArgs)
        {
            var args = cmdPrefix.Concat(extraArgs).ToArray();
            try
            {
                Process process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "adb",
                        Arguments = string.Join(" ", args),
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    }
                };
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return output.Trim();
            }
            catch (Exception ex)
            {
                throw new Exception("ADB command failed", ex);
            }
        }

        public void Connect()
        {
            Console.WriteLine("Connected to ADB.");
        }

        public void Disconnect()
        {
            Console.WriteLine($"[CONNECTION] {this.GetType().Name} is disconnected.");
        }

        public bool CheckConnectivity()
        {
            string result = RunCmd(new[] { "get-state" });
            return result.StartsWith("device");
        }

        public void SetUp()
        {
            // Initialize any setup logic, if needed
            Console.WriteLine("Setting up ADB.");
        }

        public void TearDown()
        {
            // Clean up logic
            Console.WriteLine("Tearing down ADB.");
        }

        public string GetProperty(string propertyName)
        {
            return RunCmd(new[] { "shell", "getprop", propertyName });
        }

        public string GetModelNumber()
        {
            return GetProperty(MODEL_PROPERTY);
        }

        public int GetSdkVersion()
        {
            return int.Parse(GetProperty(VERSION_SDK_PROPERTY));
        }

        public string GetReleaseVersion()
        {
            return GetProperty(VERSION_RELEASE_PROPERTY);
        }

        public int GetRoSecure()
        {
            return int.Parse(GetProperty(RO_SECURE_PROPERTY));
        }

        public int GetRoDebuggable()
        {
            return int.Parse(GetProperty(RO_DEBUGGABLE_PROPERTY));
        }

        public Dictionary<string, string> GetInstalledApps()
        {
            string result = RunCmd(new[] { "shell", "pm list packages -f" });
            var appLineRe = new Regex(@"package:(?P<apkPath>.+)=(?P<package>[^=]+)");
            var packageToPath = new Dictionary<string, string>();

            foreach (var line in result.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var match = appLineRe.Match(line);
                if (match.Success)
                {
                    packageToPath[match.Groups["package"].Value] = match.Groups["apkPath"].Value;
                }
            }

            return packageToPath;
        }

        public Dictionary<string, int> GetDisplayInfo()
        {
            string dumpsysDisplayResult = RunCmd(new[] { "shell", "dumpsys display" });
            var displayInfo = new Dictionary<string, int>();

            var logicalDisplayRe = new Regex(@"DisplayViewport{valid=true, .*orientation=(\d+), .*deviceWidth=(\d+), deviceHeight=(\d+)}");
            foreach (var line in dumpsysDisplayResult.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var match = logicalDisplayRe.Match(line);
                if (match.Success)
                {
                    displayInfo["orientation"] = int.Parse(match.Groups[1].Value);
                    displayInfo["width"] = int.Parse(match.Groups[2].Value);
                    displayInfo["height"] = int.Parse(match.Groups[3].Value);
                }
            }

            return displayInfo;
        }

        public void Touch(int x, int y, int orientation = -1)
        {
            string cmd = $"input tap {x} {y}";
            RunCmd(new[] { "shell", cmd });
        }

        public void LongTouch(int x, int y, int duration = 2000)
        {
            string cmd = $"input swipe {x} {y} {x} {y} {duration}";
            RunCmd(new[] { "shell", cmd });
        }

        public void Drag(int startX, int startY, int endX, int endY, int duration)
        {
            string cmd = $"input swipe {startX} {startY} {endX} {endY} {duration}";
            RunCmd(new[] { "shell", cmd });
        }

        public void Type(string text)
        {
            string encoded = text.Replace(" ", "%s"); // Encode spaces
            string cmd = $"input text {encoded}";
            RunCmd(new[] { "shell", cmd });
        }
    }

}
