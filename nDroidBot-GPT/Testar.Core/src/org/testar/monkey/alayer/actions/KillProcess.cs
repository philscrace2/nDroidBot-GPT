using System.Diagnostics;
using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.actions
{
    [Serializable]
    public sealed class KillProcess : Action
    {
        private readonly string processName;
        private readonly double waitSeconds;

        private KillProcess(string processName, double waitSeconds)
        {
            this.processName = processName;
            this.waitSeconds = waitSeconds;
            set(Tags.Role, Roles.System);
            set(Tags.Desc, $"Kill Process with name '{processName}'");
        }

        public static Action byName(string processName, double waitSeconds)
        {
            return new KillProcess(processName, waitSeconds);
        }

        public override void run(SUT system, State state, double duration)
        {
            if (string.IsNullOrWhiteSpace(processName))
            {
                return;
            }

            foreach (Process process in Process.GetProcesses())
            {
                try
                {
                    string name = process.ProcessName;
                    if (name.Equals(processName, StringComparison.OrdinalIgnoreCase) ||
                        name.Equals(System.IO.Path.GetFileNameWithoutExtension(processName), StringComparison.OrdinalIgnoreCase))
                    {
                        process.Kill(entireProcessTree: true);
                        process.WaitForExit(2000);
                    }
                }
                catch
                {
                    // Best effort kill action.
                }
            }

            if (waitSeconds > 0)
            {
                Util.pause(waitSeconds);
            }
        }

        public override string toShortString()
        {
            return "KillProcess";
        }

        public override string toParametersString()
        {
            return $"({processName})";
        }

        public override string toString(params Role[] discardParameters)
        {
            return ToString();
        }

        public override string ToString()
        {
            return $"Kill process '{processName}'";
        }
    }
}
