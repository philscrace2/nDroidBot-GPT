using BaseSettings = Core.nTestar.Base.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using org.testar.monkey.alayer;
using org.testar.statemodel;
using org.testar.statemodel.persistence;
using org.testar.reporting;
using TestarSettings = org.testar.settings.Settings;
using Action = org.testar.monkey.alayer.Action;
using State = org.testar.monkey.alayer.State;

namespace Core.nTestar
{
    public class DefaultProtocol : RuntimeControlsProtocol
    {
        public static bool FaultySequence { get; set; }
        public int GeneratedSequence { get; internal set; }
        public int CurrentSeq { get; internal set; }
        public int SequenceCount { get; internal set; }
        public StateModelManager StateModelManager { get; internal set; }
        public Verdict? FinalVerdict { get; internal set; }
        public static Verdict? ProcessVerdict { get; internal set; }
        public Canvas Canvas { get; internal set; }
        public int ActionCount { get; internal set; }

        protected bool LogOracleEnabled;
        protected object LogOracle;
        internal Reporting ReportManager { get; private set; } = new DummyReportManager();
        protected DateTime StartTime;
        protected object LatestState;
        public static object LastExecutedAction;

        public DefaultProtocol(BaseSettings settings) : base(settings)
        {
            this.StartTime = DateTime.Now;
            if (Enum.TryParse(typeof(Modes), settings.Get("Mode"), out var mode))
            {
                this.Mode = (Modes)mode;
            }
            else
            {
                throw new ArgumentException($"Invalid mode specified in settings: {settings.Get("Mode")}");
            }

        }

        public DefaultProtocol(BaseSettings settings, IWindowsAutomationProvider windowsAutomationProvider) : this(settings)
        {
            WindowsAutomationProvider = windowsAutomationProvider;
        }

        public IWindowsAutomationProvider? WindowsAutomationProvider { get; private set; }

        public void SetWindowsAutomationProvider(IWindowsAutomationProvider windowsAutomationProvider)
        {
            WindowsAutomationProvider = windowsAutomationProvider;
        }

        public void Run()
        {
            Console.WriteLine("Starting TESTAR protocol...");
            Initialize(Settings);
            try
            {
                switch (Mode)
                {
                    case Modes.View:
                        HandleViewMode();
                        break;
                    case Modes.Replay:
                        HandleReplayMode();
                        break;
                    case Modes.Spy:
                        HandleSpyMode();
                        break;
                    case Modes.Record:
                        Console.WriteLine("Record mode is temporarily disabled.");
                        break;
                    case Modes.Generate:
                        HandleGenerateMode();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                this.Mode = Modes.Quit;
            }
            CloseTestSession();
        }

        protected void HandleViewMode()
        {
            string filePath = Settings.Get("PathToReplaySequence");
            if (File.Exists(filePath))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            else
            {
                Console.WriteLine("Invalid file path for viewing mode.");
            }
        }

        protected void HandleReplayMode()
        {
            Console.WriteLine("Starting replay mode...");
            PreSequencePreparations();
            try
            {
                // Replay logic goes here
            }
            finally
            {
                ReportManager.finishReport();
            }
        }

        protected void HandleSpyMode()
        {
            Console.WriteLine("Starting spy mode...");
            // Spy mode logic
        }

        protected void HandleGenerateMode()
        {
            Console.WriteLine("Starting generate mode...");
            // Generate mode logic
            new GenerateMode().RunGenerateOuterLoop(this);
        }

        public override void Initialize(BaseSettings settings)
        {
            //VisualizationOn = settings.Get("VisualizeActions");
            StartTime = DateTime.Now;
            this.StartTime = DateTime.Now;
            if (Enum.TryParse(typeof(Modes), settings.Get("Mode"), out var mode))
            {
                this.Mode = (Modes)mode;
            }
            else
            {
                throw new ArgumentException($"Invalid mode specified in settings: {settings.Get("Mode")}");
            }

            InitStateModelManager(settings);
        }

        public override void CloseTestSession()
        {
            Console.WriteLine("Closing TESTAR session...");
        }

        public override void InitTestSession()
        {
            throw new NotImplementedException();
        }

        public override void PreSequencePreparations()
        {
            if (Mode != Modes.Spy)
            {
                ReportManager = new ReportManager(Mode == Modes.Replay, BuildTestarSettings());
            }
            else
            {
                ReportManager = new DummyReportManager();
            }
        }

        public override SUT StartSystem()
        {
            throw new NotImplementedException();
        }

        public override void BeginSequence(SUT system, State state)
        {
            throw new NotImplementedException();
        }

        public override State GetState(SUT system)
        {
            throw new NotImplementedException();
        }

        public override Verdict GetVerdict(State state)
        {
            throw new NotImplementedException();
        }

        public override HashSet<Action> DeriveActions(SUT system, State state)
        {
            throw new NotImplementedException();
        }

        public override Action SelectAction(State state, HashSet<Action> actions)
        {
            throw new NotImplementedException();
        }

        public override bool ExecuteAction(SUT system, State state, Action action)
        {
            throw new NotImplementedException();
        }

        public override bool MoreActions(State state)
        {
            throw new NotImplementedException();
        }

        public override bool MoreSequences()
        {
            throw new NotImplementedException();
        }

        public override void FinishSequence()
        {
            throw new NotImplementedException();
        }

        public override void StopSystem(SUT system)
        {
            throw new NotImplementedException();
        }

        public override void PostSequenceProcessing()
        {
            throw new NotImplementedException();
        }

        internal void InitGenerateMode()
        {
            var testarSettings = BuildTestarSettings();
            org.testar.OutputStructure.calculateOuterLoopDateString();
            org.testar.OutputStructure.createOutputSUTname(testarSettings);
            org.testar.OutputStructure.createOutputFolders();
        }

        private TestarSettings BuildTestarSettings()
        {
            var testarSettings = new TestarSettings();
            foreach (var kvp in Settings.Properties)
            {
                testarSettings.Set(kvp.Key, kvp.Value);
            }

            return testarSettings;
        }

        private void InitStateModelManager(BaseSettings settings)
        {
            var configTags = BuildStateModelConfigTags(settings);
            string applicationName = settings.Get("ApplicationName", string.Empty);
            string applicationVersion = settings.Get("ApplicationVersion", string.Empty);

            StateModelManager = StateModelManagerFactory.GetStateModelManager(applicationName, applicationVersion, configTags);
        }

        private static TaggableBase BuildStateModelConfigTags(BaseSettings settings)
        {
            var tags = new TaggableBase();

            tags.set(StateModelTags.StateModelEnabled, GetBoolSetting(settings, StateModelTags.StateModelEnabled.name(), false));
            tags.set(StateModelTags.DataStore, settings.Get(StateModelTags.DataStore.name(), "orientdb"));
            tags.set(StateModelTags.DataStoreType, settings.Get(StateModelTags.DataStoreType.name(), "remote"));
            tags.set(StateModelTags.DataStoreServer, settings.Get(StateModelTags.DataStoreServer.name(), string.Empty));
            tags.set(StateModelTags.DataStoreDirectory, settings.Get(StateModelTags.DataStoreDirectory.name(), string.Empty));
            tags.set(StateModelTags.DataStoreDb, settings.Get(StateModelTags.DataStoreDb.name(), string.Empty));
            tags.set(StateModelTags.DataStoreUser, settings.Get(StateModelTags.DataStoreUser.name(), string.Empty));
            tags.set(StateModelTags.DataStorePassword, settings.Get(StateModelTags.DataStorePassword.name(), string.Empty));
            tags.set(StateModelTags.DataStoreMode, settings.Get(StateModelTags.DataStoreMode.name(), PersistenceManager.DataStoreModeNone));
            tags.set(StateModelTags.ActionSelectionAlgorithm, settings.Get(StateModelTags.ActionSelectionAlgorithm.name(), "random"));
            tags.set(StateModelTags.StateModelStoreWidgets, GetBoolSetting(settings, StateModelTags.StateModelStoreWidgets.name(), false));
            tags.set(StateModelTags.ResetDataStore, GetBoolSetting(settings, StateModelTags.ResetDataStore.name(), false));

            return tags;
        }

        private static bool GetBoolSetting(BaseSettings settings, string key, bool defaultValue)
        {
            string raw = settings.Get(key, defaultValue.ToString());
            return bool.TryParse(raw, out bool parsed) ? parsed : defaultValue;
        }

        internal SUT StartSUTandLogger()
        {
            throw new NotImplementedException();
        }

        internal object GetAndStoreGeneratedSequence()
        {
            throw new NotImplementedException();
        }

        internal object GetAndStoreSequenceFile()
        {
            throw new NotImplementedException();
        }

        internal void StartTestSequence(SUT system)
        {
            throw new NotImplementedException();
        }

        internal void WriteAndCloseFragmentForReplayableSequence()
        {
            throw new NotImplementedException();
        }

        internal void ClassifyAndCopySequenceIntoAppropriateDirectory(object finalVerdict, object generatedSequence, object currentSeq)
        {
            throw new NotImplementedException();
        }

        internal void EndTestSequence()
        {
            throw new NotImplementedException();
        }

        internal void EmergencyTerminateTestSequence(SUT system, Exception e)
        {
            throw new NotImplementedException();
        }

        internal void BuildStateActionsIdentifiers(State state, HashSet<Action> actions)
        {
            throw new NotImplementedException();
        }

        internal HashSet<Action> PreSelectAction(SUT system, State state, HashSet<Action> actions)
        {
            throw new NotImplementedException();
        }

        internal void VisualizeActions(Canvas canvas, State state, HashSet<Action> actions)
        {
            throw new NotImplementedException();
        }

        internal void SaveActionIntoFragmentForReplayableSequence(Action action, State state, HashSet<Action> actions)
        {
            throw new NotImplementedException();
        }
    }

}
