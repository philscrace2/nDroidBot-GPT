using Core.nTestar.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Core.nTestar
{
    public class DefaultProtocol : RuntimeControlsProtocol
    {
        public bool FaultySequence { get; private set; }
        protected bool LogOracleEnabled;
        protected object LogOracle;
        protected object ReportManager;
        protected DateTime StartTime;
        protected object LatestState;
        public static object LastExecutedAction;

        public DefaultProtocol(Settings settings) : base(settings)
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
            // Replay logic goes here
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
        }

        protected override void Initialize(Settings settings)
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
        }

        protected override void CloseTestSession()
        {
            Console.WriteLine("Closing TESTAR session...");
        }

        public override void InitTestSession()
        {
            throw new NotImplementedException();
        }

        protected override void PreSequencePreparations()
        {
            throw new NotImplementedException();
        }

        protected override SUT StartSystem()
        {
            throw new NotImplementedException();
        }

        protected override void BeginSequence(SUT system, State state)
        {
            throw new NotImplementedException();
        }

        protected override State GetState(SUT system)
        {
            throw new NotImplementedException();
        }

        protected override Verdict GetVerdict(State state)
        {
            throw new NotImplementedException();
        }

        protected override HashSet<Action> DeriveActions(SUT system, State state)
        {
            throw new NotImplementedException();
        }

        protected override Action SelectAction(State state, HashSet<Action> actions)
        {
            throw new NotImplementedException();
        }

        protected override bool ExecuteAction(SUT system, State state, Action action)
        {
            throw new NotImplementedException();
        }

        protected override bool MoreActions(State state)
        {
            throw new NotImplementedException();
        }

        protected override bool MoreSequences()
        {
            throw new NotImplementedException();
        }

        protected override void FinishSequence()
        {
            throw new NotImplementedException();
        }

        protected override void StopSystem(SUT system)
        {
            throw new NotImplementedException();
        }

        protected override void PostSequenceProcessing()
        {
            throw new NotImplementedException();
        }
    }

}
