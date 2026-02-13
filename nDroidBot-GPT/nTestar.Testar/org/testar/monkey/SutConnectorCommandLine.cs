using System.Diagnostics;
using System.Text;
using org.testar.monkey.alayer;
using org.testar.monkey.alayer.devices;
using org.testar.monkey.alayer.exceptions;
using org.testar.settings;

namespace org.testar.monkey
{
    public class SutConnectorCommandLine
    {
        private readonly StateBuilder? builder;
        private readonly bool processListenerOracleEnabled;
        private readonly Settings settings;

        public SutConnectorCommandLine(StateBuilder? builder, bool processListenerOracleEnabled, Settings settings)
        {
            this.builder = builder;
            this.processListenerOracleEnabled = processListenerOracleEnabled;
            this.settings = settings;
        }

        public SUT startOrConnectSut()
        {
            string commandLine = settings.get(ConfigTags.SUTConnectorValue, string.Empty).Trim();
            Process? process = StartProcess(commandLine);
            if (process == null)
            {
                throw new SystemStartException($"Unable to start SUT from command line: {commandLine}");
            }

            Util.pause(settings.get(ConfigTags.StartupTime, 0.0));
            return new ConnectedSut(process, new AWTMouse(), new AWTKeyboard());
        }

        private static Process? StartProcess(string commandLine)
        {
            IReadOnlyList<string> parts = SplitCommandLine(commandLine);
            if (parts.Count == 0)
            {
                return null;
            }

            var psi = new ProcessStartInfo
            {
                FileName = parts[0],
                UseShellExecute = false
            };

            for (int i = 1; i < parts.Count; i++)
            {
                psi.ArgumentList.Add(parts[i]);
            }

            try
            {
                return Process.Start(psi);
            }
            catch
            {
                return null;
            }
        }

        private static IReadOnlyList<string> SplitCommandLine(string commandLine)
        {
            var tokens = new List<string>();
            var current = new StringBuilder();
            bool inQuotes = false;

            foreach (char c in commandLine)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }

                if (!inQuotes && char.IsWhiteSpace(c))
                {
                    if (current.Length > 0)
                    {
                        tokens.Add(current.ToString());
                        current.Clear();
                    }

                    continue;
                }

                current.Append(c);
            }

            if (current.Length > 0)
            {
                tokens.Add(current.ToString());
            }

            return tokens;
        }
    }
}
