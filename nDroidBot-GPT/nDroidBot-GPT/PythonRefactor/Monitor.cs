using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using FridaSharp;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace nDroidBot_GPT.PythonRefactor
{
    public class Monitor
    {
        private string packageName;
        private Device device;
        private List<string> sensitiveApi = new List<string>();
        private List<string> interestedApi = new List<string>();
        private List<string> methodStack = new List<string>();
        private bool attached = false;
        private int pid;
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private Session session;
        private string serial;
        private bool? firstTrigger;
        private double firstTriggerTime = 0;
        private int triggerNumber = 0;

        public string PackageName { get => packageName; set => packageName = value; }
        public string Serial { get => serial; set => serial = value; }

        public Monitor()
        {
        }

        public void SetUp()
        {
            SetLogPath();

            if (serial != null)
            {
                StartServer();
            }
            else
            {
                logger.Error("Device not found");
                return;
            }

            if (packageName != null)
            {
                Attach(packageName);
                LoadScript(session, pid);
            }
            else
            {
                logger.Error("Package not found");
            }
        }

        private void SetLogPath()
        {
            NLog.Config.LoggingConfiguration config = new NLog.Config.LoggingConfiguration();
            var logFile = new NLog.Targets.FileTarget("logfile") { FileName = "monitor.log" };
            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logFile);
            NLog.LogManager.Configuration = config;
        }

        private void StartServer()
        {
            Thread task = new Thread(StartServerTask);
            task.Start();
        }

        private void StartServerTask()
        {
            string cmd = "./droidbot/resources/start.sh";
            var processStartInfo = new ProcessStartInfo
            {
                FileName = cmd,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            var process = Process.Start(processStartInfo);
            process.WaitForExit();

            if (process.ExitCode == 1)
            {
                logger.Error("Frida-server start failed!");
                Environment.Exit(1);
            }
        }

        private void OnMessage(Message message)
        {
            if (message.Type == MessageType.Send)
            {
                var msg = message.Payload as string[];
                if (msg[0] == "SENSITIVE")
                {
                    if (!firstTrigger.HasValue)
                    {
                        firstTriggerTime = (DateTime.Now - DateTime.UtcNow).TotalMilliseconds;
                        firstTrigger = true;
                    }
                    sensitiveApi.Add(msg[1]);
                    triggerNumber++;
                }
                else
                {
                    interestedApi.Add(msg[1]);
                }
                methodStack.Add(msg[2]);
            }
            else if (message.Type == MessageType.Error)
            {
                logger.Info(message.Stack);
            }
        }

        private void Attach(string packageName)
        {
            try
            {
                device = Device.GetDeviceBySerial(serial);
                pid = device.Spawn(packageName);
                session = device.Attach(pid);
            }
            catch (Exception e)
            {
                logger.Error("[ERROR]: " + e.Message);
                logger.Info("waiting for process");
                return;
            }
            attached = true;
            logger.Info("Successfully attached to app");
        }

        private void Detach(Session session)
        {
            session.Detach();
            attached = false;
        }

        private void LoadScript(Session session, int pid)
        {
            if (attached)
            {
                string scriptDir = Path.Combine(".", "droidbot", "resources", "scripts");
                string scriptContent = BuildMonitorScript(scriptDir);
                Script script = session.CreateScript(scriptContent);
                script.On("message", (sender, message) => OnMessage(message));
                script.Load();
                device.Resume(pid);
            }
        }

        private string BuildMonitorScript(string dir, bool topdown = true)
        {
            StringBuilder script = new StringBuilder();
            foreach (var file in Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories))
            {
                script.Append(File.ReadAllText(file));
            }
            return script.ToString();
        }

        private int GetPid()
        {
            string cmd = "adb shell ps | grep " + packageName;
            var result = ExecuteCommand(cmd);
            if (result != null)
            {
                return pid;
            }
            else
            {
                cmd = "frida-ps -U";
                string tempPid = null;
                var resultLines = ExecuteCommand(cmd).Split('\n');
                foreach (var line in resultLines)
                {
                    if (packageName != null && line.Contains(packageName))
                    {
                        tempPid = line.Split("  ")[0];
                        break;
                    }
                }
                pid = int.Parse(tempPid);
            }
            return pid;
        }

        private string ExecuteCommand(string cmd)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{cmd}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var process = Process.Start(startInfo);
                using (var reader = process.StandardOutput)
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return null;
            }
        }

        private void GetDevice()
        {
            try
            {
                device = Device.GetDeviceBySerial(serial);
            }
            catch (Exception e)
            {
                logger.Error("Device not found: " + e.Message);
                device = null;
            }
        }

        private void WaitForDevices()
        {
            logger.Info("waiting for device");
            while (true)
            {
                string output = ExecuteCommand("adb -s " + serial + " shell getprop init.svc.bootanim");
                if (output.Contains("stopped"))
                {
                    break;
                }
                Thread.Sleep(3000);
            }
        }

        public void CheckEnv()
        {
            GetPid();
            GetDevice();
            if (device == null)
            {
                WaitForDevices();
                SetUp();
                return;
            }

            if (pid == 0)
            {
                if (attached)
                {
                    Detach(session);
                }
            }
            else
            {
                if (!attached)
                {
                    Attach(packageName);
                    LoadScript(session, pid);
                }
            }
        }

        public List<string> GetSensitiveApi()
        {
            List<string> tempState = new List<string>(sensitiveApi);
            sensitiveApi.Clear();
            return tempState;
        }

        public List<string> GetInterestedApi()
        {
            List<string> tempState = new List<string>(interestedApi);
            interestedApi.Clear();
            return tempState;
        }

        public List<string> GetMethodStackApi()
        {
            List<string> tempState = new List<string>(methodStack);
            methodStack.Clear();
            return tempState;
        }

        public double GetFirstTriggerTime()
        {
            return firstTriggerTime;
        }

        public int GetTriggerNumber()
        {
            return triggerNumber;
        }

        public void Stop()
        {
            Detach(session);
            logger.Info("Stopped monitor...");
        }
    }

}
