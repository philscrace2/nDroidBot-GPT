using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using org.testar.monkey.alayer;
using org.testar.monkey.alayer.actions;
using org.testar.monkey.alayer.devices;
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
            string sequenceName = $"{OutputStructure.startInnerLoopDateString ?? DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}_{OutputStructure.executedSUTname ?? "sut"}_sequence_{OutputStructure.sequenceInnerLoopCount}";
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

            string connector = settingsRef().Get("SUTConnector", "COMMAND_LINE").Trim().ToUpperInvariant();
            string connectorValue = settingsRef().Get("SUTConnectorValue", string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(connectorValue))
            {
                if (connector == "COMMAND_LINE")
                {
                    sut.AttachProcess(StartProcessFromCommandLine(connectorValue));
                }
                else if (connector == "SUT_PROCESS_NAME")
                {
                    string processName = Path.GetFileNameWithoutExtension(connectorValue);
                    sut.AttachProcess(Process.GetProcessesByName(processName).FirstOrDefault());
                }
                else if (connector == "SUT_WINDOW_TITLE")
                {
                    sut.AttachProcess(Process.GetProcesses().FirstOrDefault(p =>
                    {
                        try
                        {
                            return !string.IsNullOrWhiteSpace(p.MainWindowTitle) &&
                                   p.MainWindowTitle.Contains(connectorValue, StringComparison.OrdinalIgnoreCase);
                        }
                        catch
                        {
                            return false;
                        }
                    }));
                }
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
            SetStateScreenshot(state);
            reportManager.addState(state);
            return state;
        }

        protected override Verdict getVerdict(State state)
        {
            Verdict verdict = state.get(Tags.OracleVerdict, Verdict.OK);
            if (verdict.severity() > finalVerdict.severity())
            {
                finalVerdict = verdict;
            }

            return verdict;
        }

        protected override ISet<Action> deriveActions(SUT system, State state)
        {
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
            return actionCount < sequenceLength && (DateTime.UtcNow - startTimeUtc).TotalSeconds < maxTime;
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

        private sealed class DummySut : TaggableBase, SUT
        {
            private Process? process;

            public void AttachProcess(Process? runningProcess)
            {
                process = runningProcess;
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
        }
    }
}
