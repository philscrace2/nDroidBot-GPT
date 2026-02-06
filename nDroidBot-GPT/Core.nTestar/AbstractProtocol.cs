using System;
using System.Collections.Generic;
using org.testar.monkey.alayer;
using Action = org.testar.monkey.alayer.Action;
using State = org.testar.monkey.alayer.State;
using SUT = org.testar.monkey.alayer.SUT;
using Verdict = org.testar.monkey.alayer.Verdict;
using BaseSettings = Core.nTestar.Base.Settings;

namespace Core.nTestar
{
    public abstract class AbstractProtocol
    {
        public BaseSettings Settings { get; private set; }

        public abstract void Initialize(BaseSettings settings);
        public abstract void InitTestSession();
        public abstract void PreSequencePreparations();
        public abstract SUT StartSystem();
        public abstract void BeginSequence(SUT system, State state);
        public abstract State GetState(SUT system);
        public abstract Verdict GetVerdict(State state);
        public abstract HashSet<Action> DeriveActions(SUT system, State state);
        public abstract Action SelectAction(State state, HashSet<Action> actions);
        public abstract bool ExecuteAction(SUT system, State state, Action action);
        public abstract bool MoreActions(State state);
        public abstract bool MoreSequences();
        public abstract void FinishSequence();
        public abstract void StopSystem(SUT system);
        public abstract void PostSequenceProcessing();
        public abstract void CloseTestSession();
    }


}
