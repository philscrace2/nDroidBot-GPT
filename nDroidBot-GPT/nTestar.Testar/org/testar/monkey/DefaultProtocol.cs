using System;
using System.Collections.Generic;
using System.Linq;
using org.testar.monkey.alayer;
using org.testar.monkey.alayer.actions;
using org.testar.monkey.alayer.devices;
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
        }

        protected override void preSequencePreparations()
        {
            cv = windowsAutomationProvider?.CreateCanvas(Pen.PEN_BLACK);
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
            return sut;
        }

        protected override void beginSequence(SUT system, State state)
        {
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
            return state;
        }

        protected override Verdict getVerdict(State state)
        {
            return state.get(Tags.OracleVerdict, Verdict.OK);
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

            if (actions.Count == 0)
            {
                actions.Add(new NOP());
            }

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
            return actionCount < sequenceLength;
        }

        protected override bool moreSequences()
        {
            int sequences = ReadIntSetting("Sequences", 1);
            return sequenceCount < sequences;
        }

        protected override void finishSequence()
        {
        }

        protected override void stopSystem(SUT system)
        {
        }

        protected override void postSequenceProcessing()
        {
        }

        protected override void closeTestSession()
        {
        }

        protected virtual ISet<Action> preSelectAction(SUT system, State state, ISet<Action> actions)
        {
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

        private sealed class DummySut : TaggableBase, SUT
        {
        }
    }
}
