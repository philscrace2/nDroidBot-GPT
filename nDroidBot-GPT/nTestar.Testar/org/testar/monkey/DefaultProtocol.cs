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
        private StateBuilder? builder;
        private IWindowsAutomationProvider? windowsAutomationProvider;
        protected Reporting reportManager = new DummyReportManager();
        protected Verdict finalVerdict = Verdict.OK;
        private string? sequenceLogFilePath;
        protected List<ProcessInfo>? contextRunningProcesses;
        protected bool processListenerOracleEnabled;

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
            builder = stateBuilder;
            processListenerOracleEnabled = settings.get(ConfigTags.ProcessListenerEnabled, false);

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
            contextRunningProcesses = SystemProcessHandling.getRunningProcesses("START");
            try
            {
                foreach (string d in settingsRef().get(ConfigTags.Delete, new List<string>()))
                {
                    Util.delete(d);
                }

                foreach (Pair<string, string> fromTo in settingsRef().get(ConfigTags.CopyFromTo, new List<Pair<string, string>>()))
                {
                    Util.copyToDirectory(fromTo.left(), fromTo.right());
                }
            }
            catch (IOException ioe)
            {
                throw new SystemStartException(ioe.Message);
            }

            string sutConnectorType = settingsRef().get(ConfigTags.SUTConnector, Settings.SUT_CONNECTOR_COMMAND_LINE);
            string connectorValue = settingsRef().get(ConfigTags.SUTConnectorValue, string.Empty);
            if (string.IsNullOrEmpty(connectorValue))
            {
                    string msg = "It seems that the SUTConnectorValue setting is null or empty!\n" +
                             "Please provide a valid value for the SUTConnector: " + sutConnectorType;
                popupMessage(msg);
                throw new SystemStartException(msg);
            }

            if (sutConnectorType.Equals(Settings.SUT_CONNECTOR_WINDOW_TITLE, StringComparison.Ordinal))
            {
                var sutConnector = new SutConnectorWindowTitle(
                    settingsRef().get(ConfigTags.SUTConnectorValue, string.Empty),
                    (long)Math.Round(settingsRef().get(ConfigTags.StartupTime, 0.0) * 1000.0),
                    builder,
                    settingsRef().get(ConfigTags.ForceForeground, true));
                return sutConnector.startOrConnectSut();
            }
            else if (sutConnectorType.StartsWith(Settings.SUT_CONNECTOR_PROCESS_NAME, StringComparison.Ordinal))
            {
                var sutConnector = new SutConnectorProcessName(
                    settingsRef().get(ConfigTags.SUTConnectorValue, string.Empty),
                    (long)Math.Round(settingsRef().get(ConfigTags.StartupTime, 0.0) * 1000.0));
                return sutConnector.startOrConnectSut();
            }
            else
            {
                var sutConnector = new SutConnectorCommandLine(builder, processListenerOracleEnabled, settingsRef());
                return sutConnector.startOrConnectSut();
            }
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
            if (system is ConnectedSut sut)
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
            if (system is ConnectedSut sut)
            {
                return sut.IsRunning;
            }

            return true;
        }

        private static bool IsSystemNotResponding(SUT system)
        {
            if (system is ConnectedSut sut)
            {
                return sut.IsNotResponding;
            }

            return false;
        }

        private static bool IsSystemForeground(SUT system)
        {
            if (system is ConnectedSut sut)
            {
                return sut.IsForeground;
            }

            return true;
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

        private static void popupMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
