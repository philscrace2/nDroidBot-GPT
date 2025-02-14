using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using DroidBot.InputEvent;

namespace nDroidBot_GPT.PythonRefactor
{
    public class UnknownInputException : Exception { }

    public class InputManager
    {
        private static readonly string DEFAULT_POLICY = "POLICY_GREEDY_DFS";
        private static readonly int DEFAULT_EVENT_INTERVAL = 1;
        private static readonly int DEFAULT_EVENT_COUNT = 100000000;
        private static readonly int DEFAULT_TIMEOUT = -1;

        private readonly Device device;
        private readonly App app;
        private readonly string task;
        private readonly string policyName;
        private readonly bool randomInput;
        private readonly int eventCount;
        private readonly int eventInterval;
        private readonly string replayOutput;
        private readonly string profilingMethod;
        private List<Event> events;
        private IInputPolicy policy;
        private DroidBotScript script;
        private bool enabled;
        private Process monkey;

        public InputManager(Device device, App app, string task, string policyName, bool randomInput,
                            int eventCount, int eventInterval, string scriptPath = null, string profilingMethod = null,
                            string master = null, string replayOutput = null)
        {
            this.device = device;
            this.app = app;
            this.task = task;
            this.policyName = policyName;
            this.randomInput = randomInput;
            this.eventCount = eventCount;
            this.eventInterval = eventInterval;
            this.replayOutput = replayOutput;
            this.profilingMethod = profilingMethod;
            this.enabled = true;
            this.events = new List<Event>();

            if (!string.IsNullOrEmpty(scriptPath))
            {
                var scriptDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(scriptPath));
                this.script = new DroidBotScript(scriptDict);
            }

            this.policy = GetInputPolicy(device, app, master);
        }

        private IInputPolicy GetInputPolicy(Device device, App app, string master)
        {
            IInputPolicy inputPolicy = null;

            switch (policyName)
            {
                case "POLICY_NONE":
                    break;

                case "POLICY_MONKEY":
                    break;

                case "POLICY_NAIVE_DFS":
                case "POLICY_NAIVE_BFS":
                    inputPolicy = new UtgNaiveSearchPolicy(device, app, randomInput, policyName);
                    break;

                case "POLICY_GREEDY_DFS":
                case "POLICY_GREEDY_BFS":
                    inputPolicy = new UtgGreedySearchPolicy(device, app, randomInput, policyName);
                    break;

                case "POLICY_REPLAY":
                    inputPolicy = new UtgReplayPolicy(device, app, replayOutput);
                    break;

                case "POLICY_MANUAL":
                    inputPolicy = new ManualPolicy(device, app);
                    break;

                case "POLICY_TASK":
                    inputPolicy = new TaskPolicy(device, app, randomInput, task);
                    break;

                default:
                    Console.WriteLine("No valid input policy specified. Using policy \"none\".");
                    break;
            }

            if (inputPolicy is UtgBasedInputPolicy utgPolicy)
            {
                utgPolicy.Script = script;
                utgPolicy.Master = master;
            }

            return inputPolicy;
        }

        public void AddEvent(Event eventToAdd)
        {
            if (eventToAdd == null)
                return;

            events.Add(eventToAdd);

            var eventLog = new EventLog(device, app, eventToAdd, profilingMethod);
            eventLog.Start();

            while (true)
            {
                Thread.Sleep(eventInterval);
                if (!device.PauseSendingEvent)
                {
                    break;
                }
            }

            eventLog.Stop();
        }

        public void Start()
        {
            Console.WriteLine($"Start sending events, policy is {policyName}");

            try
            {
                if (policy != null)
                {
                    policy.Start(this);
                }
                else if (policyName == "POLICY_NONE")
                {
                    device.StartApp(app);
                    if (eventCount == 0) return;

                    while (enabled)
                    {
                        Thread.Sleep(1000);
                    }
                }
                else if (policyName == "POLICY_MONKEY")
                {
                    var throttle = eventInterval * 1000;
                    var monkeyCmd = $"adb -s {device.Serial} shell monkey {(string.IsNullOrEmpty(app.GetPackageName()) ? "" : "-p " + app.GetPackageName())} --ignore-crashes --ignore-security-exceptions --throttle {throttle} -v {eventCount}";

                    monkey = Process.Start(new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = "/c " + monkeyCmd,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });

                    foreach (var line in monkey.StandardOutput.ReadLines())
                    {
                        Console.WriteLine(line);
                    }

                    monkey.WaitForExit();
                }
                else if (policyName == "POLICY_MANUAL")
                {
                    device.StartApp(app);
                    while (enabled)
                    {
                        var input = Console.ReadLine();
                        if (input.StartsWith("q"))
                        {
                            break;
                        }

                        var state = device.GetCurrentState();
                        if (state != null)
                        {
                            state.SaveToDir();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }

            Stop();
            Console.WriteLine("Finished sending events.");
        }

        public void Stop()
        {
            if (monkey != null && monkey.HasExited == false)
            {
                monkey.Kill();
            }

            enabled = false;
        }
    }
}

