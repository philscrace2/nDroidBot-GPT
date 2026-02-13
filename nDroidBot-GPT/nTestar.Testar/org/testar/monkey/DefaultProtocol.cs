using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using org.testar.monkey.alayer;
using org.testar.monkey.alayer.actions;
using org.testar.monkey.alayer.devices;
using org.testar.monkey.alayer.exceptions;
using org.testar.reporting;
using org.testar.serialisation;
using org.testar.settings;
using org.testar.stub;
using Action = org.testar.monkey.alayer.Action;
using State = org.testar.monkey.alayer.State;

namespace org.testar.monkey
{
    public class DefaultProtocol : RuntimeControlsProtocol
    {
        protected org.testar.monkey.alayer.devices.Mouse? mouse;
        protected Canvas? cv;
        private State? stateForClickFilterLayerProtocol;
        protected int actionCount;
        protected int sequenceCount;
        protected DateTime startTimeUtc;
        private StateBuilder? stateBuilder;
        private IWindowsAutomationProvider? windowsAutomationProvider;
        protected Reporting reportManager = new DummyReportManager();
        protected Verdict finalVerdict = Verdict.OK;
        private string? sequenceLogFilePath;

        public static Func<IWindowsAutomationProvider?>? WindowsAutomationProviderFactory { get; set; }

        public DefaultProtocol() : this(new Settings())
        {
        }

        public DefaultProtocol(Settings settings) : base(settings)
        {
            startTimeUtc = DateTime.UtcNow;
        }

        public State? getStateForClickFilterLayerProtocol()
        {
            return stateForClickFilterLayerProtocol;
        }

        public void setStateForClickFilterLayerProtocol(State? state)
        {
            stateForClickFilterLayerProtocol = state;
        }

        public void Run()
        {
            Run(settingsRef());
        }

        public void Run(Settings settings)
        {
            run(settings);
        }

        public void run(Settings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            this.settings = settings;
            initialize(settings);
            initTestSession();

            while (mode() != Modes.Quit && moreSequences())
            {
                SUT? system = null;
                try
                {
                    preSequencePreparations();
                    system = startSystem();

                    State state = getState(system);
                    setStateForClickFilterLayerProtocol(state);
                    beginSequence(system, state);

                    while (mode() != Modes.Quit && moreActions(state))
                    {
                        state = getState(system);
                        setStateForClickFilterLayerProtocol(state);
                        _ = getVerdict(state);

                        ISet<Action> actions = deriveActions(system, state);
                        actions = preSelectAction(system, state, actions);
                        if (actions.Count == 0)
                        {
                            break;
                        }

                        if (VisualizationOn && cv != null)
                        {
                            visualizeActions(cv, state, actions);
                        }

                        Action action = selectAction(state, actions);
                        bool executed = executeAction(system, state, action);
                        if (executed)
                        {
                            actionCount++;
                        }
                    }

                    finishSequence();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"DefaultProtocol run failed in sequence {sequenceCount}: {ex.Message}");
                }
                finally
                {
                    if (system != null)
                    {
                        stopSystem(system);
                    }

                    postSequenceProcessing();
                    sequenceCount++;
                    actionCount = 0;
                }
            }

            closeTestSession();
        }

        protected override void initialize(Settings settings)
        {
            this.settings = settings;
            startTimeUtc = DateTime.UtcNow;
            actionCount = 0;
            sequenceCount = 0;
            windowsAutomationProvider = WindowsAutomationProviderFactory?.Invoke();
            stateBuilder = windowsAutomationProvider?.CreateStateBuilder();

            string modeString = settings.Get("Mode", Modes.Generate.ToString());
            if (Enum.TryParse(modeString, true, out Modes parsedMode))
            {
                Mode = parsedMode;
            }
            else
            {
                Mode = Modes.Generate;
            }
        }

        protected override void initTestSession()
        {
            Main.outputDir = settingsRef().Get("OutputDir", Main.outputDir);
            Directory.CreateDirectory(Main.outputDir);

            if (mode() == Modes.Generate || mode() == Modes.Replay)
            {
                OutputStructure.calculateOuterLoopDateString();
                OutputStructure.sequenceInnerLoopCount = 0;
                OutputStructure.createOutputSUTname(settingsRef());
                OutputStructure.createOutputFolders();
            }
        }

        protected override void preSequencePreparations()
        {
            cv = windowsAutomationProvider?.CreateCanvas(Pen.PEN_BLACK);
            reportManager = mode() == Modes.Spy ? new DummyReportManager() : new ReportManager(mode() == Modes.Replay, settingsRef());

            OutputStructure.sequenceInnerLoopCount++;
            OutputStructure.calculateInnerLoopDateString();

            string logsDir = OutputStructure.logsOutputDir ?? Main.outputDir;
            Directory.CreateDirectory(logsDir);
            string innerLoopTimestamp = OutputStructure.startInnerLoopDateString ?? DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss");
            string sequenceName = $"{innerLoopTimestamp}_{OutputStructure.executedSUTname ?? "sut"}_sequence_{OutputStructure.sequenceInnerLoopCount}";
            sequenceLogFilePath = Path.Combine(logsDir, sequenceName + ".log");

            var stream = new StreamWriter(new FileStream(sequenceLogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read));
            LogSerialiser.Start(stream, ReadIntSetting("LogLevel", 1));

            if (!string.IsNullOrWhiteSpace(OutputStructure.screenshotsOutputDir))
            {
                ScreenshotSerialiser.Start(OutputStructure.screenshotsOutputDir!, sequenceName);
            }
        }

        protected override SUT startSystem()
        {
            try
            {
                PrepareSutEnvironment();
            }
            catch (Exception ex)
            {
                throw new SystemStartException(ex.Message);
            }

            string connector = settingsRef().Get("SUTConnector", Settings.SUT_CONNECTOR_COMMAND_LINE).Trim().ToUpperInvariant();
            string connectorValue = settingsRef().Get("SUTConnectorValue", string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(connectorValue))
            {
                throw new SystemStartException($"SUTConnectorValue is null or empty for connector '{connector}'.");
            }

            var sut = new DummySut();
            if (mouse == null)
            {
                mouse = new AWTMouse();
            }

            sut.set(Tags.StandardMouse, mouse);
            sut.set(Tags.HasStandardMouse, true);
            sut.set(Tags.StandardKeyboard, new AWTKeyboard());
            sut.set(Tags.Title, settingsRef().Get("ApplicationName", "SUT"));
            sut.set(Tags.Role, Roles.SUT);

            if (connector == Settings.SUT_CONNECTOR_WINDOW_TITLE)
            {
                Process? connected = FindProcessByWindowTitle(connectorValue);
                if (connected == null)
                {
                    throw new SystemStartException($"Could not find a process with window title containing '{connectorValue}'.");
                }

                sut.AttachProcess(connected);
            }
            else if (connector.StartsWith(Settings.SUT_CONNECTOR_PROCESS_NAME, StringComparison.Ordinal))
            {
                string processName = Path.GetFileNameWithoutExtension(connectorValue);
                Process? connected = Process.GetProcessesByName(processName).FirstOrDefault();
                if (connected == null)
                {
                    throw new SystemStartException($"Could not find a running process named '{processName}'.");
                }

                sut.AttachProcess(connected);
            }
            else
            {
                Process? launched = StartProcessFromCommandLine(connectorValue);
                if (launched == null)
                {
                    throw new SystemStartException($"Could not start command line SUT '{connectorValue}'.");
                }

                sut.AttachProcess(launched);
            }

            WaitForStartupDelay();
            return sut;
        }

        protected override void beginSequence(SUT system, State state)
        {
            finalVerdict = Verdict.OK;
            actionCount = 0;
        }

        protected override State getState(SUT system)
        {
            State state;
            if (stateBuilder != null)
            {
                state = stateBuilder.apply(system);
            }
            else
            {
                var fallback = new StateStub();
                fallback.set(Tags.ConcreteID, $"state-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}");
                fallback.set(Tags.Role, Roles.System);
                state = fallback;
            }

            state.set(Tags.OracleVerdict, Verdict.OK);
            state.set(Tags.TimeStamp, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            state.set(Tags.MaxZIndex, 0.0);
            state.set(Tags.IsRunning, IsSystemRunning(system));
            state.set(Tags.NotResponding, IsSystemNotResponding(system));
            state.set(Tags.Foreground, IsSystemForeground(system));
            state.set(Tags.RunningProcesses, GetRunningProcesses());
            SetStateScreenshot(state);
            reportManager.addState(state);
            return state;
        }

        protected override Verdict getVerdict(State state)
        {
            Verdict verdict = state.get(Tags.OracleVerdict, Verdict.OK);
            if (!state.get(Tags.IsRunning, true))
            {
                verdict = verdict.join(new Verdict(Verdict.Severity.FAIL, "System is offline! Closed unexpectedly."));
            }

            if (state.get(Tags.NotResponding, false))
            {
                verdict = verdict.join(new Verdict(Verdict.Severity.FAIL, "System is unresponsive."));
            }

            state.set(Tags.OracleVerdict, verdict);
            if (verdict.severity() > finalVerdict.severity())
            {
                finalVerdict = verdict;
            }

            return verdict;
        }

        protected override ISet<Action> deriveActions(SUT system, State state)
        {
            var systemActions = DeriveSystemActions(system, state);
            if (systemActions.Count > 0)
            {
                reportManager.addActions(systemActions);
                return systemActions;
            }

            var actions = new HashSet<Action>();
            var compiler = new AnnotatingActionCompiler();

            foreach (Widget widget in state)
            {
                if (ReferenceEquals(widget, state))
                {
                    continue;
                }

                if (!widget.get(Tags.Enabled, true) || widget.get(Tags.Blocked, false))
                {
                    continue;
                }

                Shape shape = widget.get(Tags.Shape, Rect.from(0, 0, 0, 0));
                if (shape.width() <= 1 || shape.height() <= 1)
                {
                    continue;
                }

                try
                {
                    if (IsTypeable(widget))
                    {
                        actions.Add(compiler.clickTypeInto(widget, "test", true));
                    }
                    else
                    {
                        actions.Add(compiler.leftClickAt(widget));
                    }
                }
                catch
                {
                    // Skip widgets that cannot build actions yet.
                }

                if (actions.Count >= 200)
                {
                    break;
                }
            }

            reportManager.addActions(actions);
            return actions;
        }

        protected override Action selectAction(State state, ISet<Action> actions)
        {
            if (actions.Count == 0)
            {
                return new NOP();
            }

            Action? selected = org.testar.RandomActionSelector.selectRandomAction(actions);
            return selected ?? actions.First();
        }

        protected override bool executeAction(SUT system, State state, Action action)
        {
            if (action == null)
            {
                return false;
            }

            try
            {
                reportManager.addSelectedAction(state, action);
                action.run(system, state, 0);
                double waitAfterAction = ReadDoubleSetting("TimeToWaitAfterAction", 0.0);
                Util.pause(waitAfterAction);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Action execution failed: {ex.Message}");
                return false;
            }
        }

        protected override bool moreActions(State state)
        {
            int sequenceLength = ReadIntSetting("SequenceLength", 100);
            double maxTime = ReadDoubleSetting("MaxTime", double.MaxValue);
            return state.get(Tags.IsRunning, true) &&
                   !state.get(Tags.NotResponding, false) &&
                   actionCount < sequenceLength &&
                   (DateTime.UtcNow - startTimeUtc).TotalSeconds < maxTime;
        }

        protected override bool moreSequences()
        {
            int sequences = ReadIntSetting("Sequences", 1);
            double maxTime = ReadDoubleSetting("MaxTime", double.MaxValue);
            return sequenceCount < sequences && (DateTime.UtcNow - startTimeUtc).TotalSeconds < maxTime;
        }

        protected override void finishSequence()
        {
            cv?.release();
            cv = null;
        }

        protected override void stopSystem(SUT system)
        {
            if (system is DummySut sut)
            {
                sut.Stop();
            }
        }

        protected override void postSequenceProcessing()
        {
            reportManager.addTestVerdict(finalVerdict);
            reportManager.finishReport();
            ScreenshotSerialiser.Finish();
            ScreenshotSerialiser.Exit();
            LogSerialiser.Finish();
            LogSerialiser.Exit();
        }

        protected override void closeTestSession()
        {
            ScreenshotSerialiser.Finish();
            ScreenshotSerialiser.Exit();
            LogSerialiser.Finish();
            LogSerialiser.Exit();
        }

        protected virtual ISet<Action> preSelectAction(SUT system, State state, ISet<Action> actions)
        {
            if (actions.Count == 0)
            {
                Action escAction = new AnnotatingActionCompiler().hitKey(KBKeys.VK_ESCAPE);
                escAction.mapOriginWidget(state);
                var forced = new HashSet<Action> { escAction };
                reportManager.addActions(forced);
                return forced;
            }

            Action? systemAction = actions.FirstOrDefault(a => Role.isOneOf(a.get(Tags.Role, Roles.Widget), Roles.System));
            if (systemAction != null)
            {
                var forced = new HashSet<Action> { systemAction };
                reportManager.addActions(forced);
                return forced;
            }

            return actions;
        }

        protected virtual void visualizeActions(Canvas canvas, State state, ISet<Action> actions)
        {
        }

        private int ReadIntSetting(string key, int fallback)
        {
            string raw = settingsRef().Get(key, fallback.ToString());
            return int.TryParse(raw, out int value) ? value : fallback;
        }

        private double ReadDoubleSetting(string key, double fallback)
        {
            string raw = settingsRef().Get(key, fallback.ToString(System.Globalization.CultureInfo.InvariantCulture));
            return double.TryParse(raw, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double value)
                ? value
                : fallback;
        }

        private static bool IsTypeable(Widget widget)
        {
            string roleName = widget.get(Tags.Role, Roles.Widget).name();
            return roleName.Contains("Edit", StringComparison.OrdinalIgnoreCase) ||
                   roleName.Contains("Text", StringComparison.OrdinalIgnoreCase) ||
                   roleName.Contains("Document", StringComparison.OrdinalIgnoreCase);
        }

        private ISet<Action> DeriveSystemActions(SUT system, State state)
        {
            var actions = new HashSet<Action>();
            long sutPid = system.get(Tags.PID, 0L);
            string processRegex = settingsRef().Get("ProcessesToKillDuringTest", string.Empty);
            if (!string.IsNullOrWhiteSpace(processRegex))
            {
                Regex regex;
                try
                {
                    regex = new Regex(processRegex, RegexOptions.CultureInvariant);
                }
                catch
                {
                    regex = new Regex("$a", RegexOptions.CultureInvariant);
                }

                foreach (Pair<long, string> process in state.get(Tags.RunningProcesses, new List<Pair<long, string>>()))
                {
                    if (process.left() != sutPid && regex.IsMatch(process.right()))
                    {
                        Action kill = KillProcess.byName(process.right(), 0);
                        kill.mapOriginWidget(state);
                        actions.Add(kill);
                        return actions;
                    }
                }
            }

            if (!state.get(Tags.Foreground, true))
            {
                var activate = new ActivateSystem();
                activate.mapOriginWidget(state);
                actions.Add(activate);
                return actions;
            }

            return actions;
        }

        private static List<Pair<long, string>> GetRunningProcesses()
        {
            var running = new List<Pair<long, string>>();
            foreach (Process process in Process.GetProcesses())
            {
                try
                {
                    running.Add(Pair<long, string>.from(process.Id, process.ProcessName));
                }
                catch
                {
                    // Ignore protected processes.
                }
            }

            return running;
        }

        private static bool IsSystemRunning(SUT system)
        {
            if (system is DummySut sut)
            {
                return sut.IsRunning;
            }

            return true;
        }

        private static bool IsSystemNotResponding(SUT system)
        {
            if (system is DummySut sut)
            {
                return sut.IsNotResponding;
            }

            return false;
        }

        private static bool IsSystemForeground(SUT system)
        {
            if (system is DummySut sut)
            {
                return sut.IsForeground;
            }

            return true;
        }

        private void WaitForStartupDelay()
        {
            double startupSeconds = ReadDoubleSetting("StartupTime", 0.0);
            if (startupSeconds > 0)
            {
                Util.pause(startupSeconds);
            }
        }

        private static Process? StartProcessFromCommandLine(string commandLine)
        {
            IReadOnlyList<string> parts = SplitCommandLine(commandLine);
            if (parts.Count == 0)
            {
                return null;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = parts[0],
                UseShellExecute = false
            };

            for (int i = 1; i < parts.Count; i++)
            {
                startInfo.ArgumentList.Add(parts[i]);
            }

            try
            {
                return Process.Start(startInfo);
            }
            catch
            {
                return null;
            }
        }

        private static Process? FindProcessByWindowTitle(string title)
        {
            return Process.GetProcesses().FirstOrDefault(p =>
            {
                try
                {
                    return !string.IsNullOrWhiteSpace(p.MainWindowTitle) &&
                           p.MainWindowTitle.Contains(title, StringComparison.OrdinalIgnoreCase);
                }
                catch
                {
                    return false;
                }
            });
        }

        private static IReadOnlyList<string> SplitCommandLine(string commandLine)
        {
            var tokens = new List<string>();
            var current = new System.Text.StringBuilder();
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

        private void SetStateScreenshot(State state)
        {
            if (mode() == Modes.Spy)
            {
                return;
            }

            string stateId = state.get(Tags.ConcreteID, $"state-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}");
            string path = ScreenshotSerialiser.SaveStateshot(stateId, new AWTCanvas());
            state.set(Tags.ScreenshotPath, path);
        }

        private void PrepareSutEnvironment()
        {
            foreach (string path in ParseListSetting("Delete"))
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, recursive: true);
                }
                else if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }

            foreach ((string from, string to) in ParseCopyFromTo())
            {
                CopyPathToDirectory(from, to);
            }
        }

        private IEnumerable<string> ParseListSetting(string key)
        {
            string raw = settingsRef().Get(key, string.Empty);
            return raw.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => s.Length > 0);
        }

        private IEnumerable<(string from, string to)> ParseCopyFromTo()
        {
            string raw = settingsRef().Get("CopyFromTo", string.Empty);
            foreach (string entry in raw.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                string[] parts = entry.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (parts.Length == 2)
                {
                    yield return (parts[0], parts[1]);
                }
            }
        }

        private static void CopyPathToDirectory(string from, string toDirectory)
        {
            if (File.Exists(from))
            {
                Directory.CreateDirectory(toDirectory);
                string fileName = Path.GetFileName(from);
                File.Copy(from, Path.Combine(toDirectory, fileName), overwrite: true);
                return;
            }

            if (!Directory.Exists(from))
            {
                return;
            }

            Directory.CreateDirectory(toDirectory);
            foreach (string sourceFile in Directory.EnumerateFiles(from, "*", SearchOption.AllDirectories))
            {
                string relative = Path.GetRelativePath(from, sourceFile);
                string destination = Path.Combine(toDirectory, relative);
                Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
                File.Copy(sourceFile, destination, overwrite: true);
            }
        }

        private sealed class DummySut : TaggableBase, SUT
        {
            private Process? process;
            private readonly int currentProcessId = Environment.ProcessId;

            public bool IsRunning => process == null || !process.HasExited;
            public bool IsNotResponding
            {
                get
                {
                    if (process == null || process.HasExited || !OperatingSystem.IsWindows())
                    {
                        return false;
                    }

                    try
                    {
                        return !process.Responding;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            public bool IsForeground
            {
                get
                {
                    if (!OperatingSystem.IsWindows())
                    {
                        return true;
                    }

                    int processId = process?.Id ?? currentProcessId;
                    return IsForegroundProcess(processId);
                }
            }

            public void AttachProcess(Process? runningProcess)
            {
                process = runningProcess;
                long pid = process?.Id ?? currentProcessId;
                set(Tags.PID, pid);
                set(Tags.SystemActivator, true);
                set(Tags.IsRunning, process == null || !process.HasExited);
            }

            public void Stop()
            {
                if (process == null)
                {
                    return;
                }

                try
                {
                    if (!process.HasExited)
                    {
                        process.Kill(entireProcessTree: true);
                        process.WaitForExit(2000);
                    }
                }
                catch
                {
                    // Best effort stop.
                }
            }

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            private static extern IntPtr GetForegroundWindow();

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

            private static bool IsForegroundProcess(int pid)
            {
                IntPtr hwnd = GetForegroundWindow();
                if (hwnd == IntPtr.Zero)
                {
                    return true;
                }

                _ = GetWindowThreadProcessId(hwnd, out uint foregroundPid);
                return foregroundPid == pid;
            }
        }
    }
}
