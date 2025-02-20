using System;
using System.Collections.Generic;
using Core.nTestar.Base;

namespace Core.nTestar
{
    public abstract class AbstractProtocol
    {
        protected Settings Settings { get; private set; }

        protected abstract void Initialize(Settings settings);
        protected abstract void InitTestSession();
        protected abstract void PreSequencePreparations();
        protected abstract SUT StartSystem();
        protected abstract void BeginSequence(SUT system, State state);
        protected abstract State GetState(SUT system);
        protected abstract Verdict GetVerdict(State state);
        protected abstract HashSet<Action> DeriveActions(SUT system, State state);
        protected abstract Action SelectAction(State state, HashSet<Action> actions);
        protected abstract bool ExecuteAction(SUT system, State state, Action action);
        protected abstract bool MoreActions(State state);
        protected abstract bool MoreSequences();
        protected abstract void FinishSequence();
        protected abstract void StopSystem(SUT system);
        protected abstract void PostSequenceProcessing();
        protected abstract void CloseTestSession();
    }


    public class SUT { }
    public class State { }
    public class Verdict { }
    public class Action { }

}
