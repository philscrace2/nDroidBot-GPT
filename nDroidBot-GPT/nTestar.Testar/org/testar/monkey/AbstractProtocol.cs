using System.Collections.Generic;
using org.testar.monkey.alayer;
using org.testar.settings;

namespace org.testar.monkey
{
    public abstract class AbstractProtocol
    {
        protected Settings settings;

        protected Settings settingsRef()
        {
            return settings;
        }

        protected abstract void initialize(Settings settings);
        protected abstract void initTestSession();
        protected abstract void preSequencePreparations();
        protected abstract SUT startSystem();
        protected abstract void beginSequence(SUT system, State state);
        protected abstract State getState(SUT system);
        protected abstract Verdict getVerdict(State state);
        protected abstract ISet<Action> deriveActions(SUT system, State state);
        protected abstract Action selectAction(State state, ISet<Action> actions);
        protected abstract bool executeAction(SUT system, State state, Action action);
        protected abstract bool moreActions(State state);
        protected abstract bool moreSequences();
        protected abstract void finishSequence();
        protected abstract void stopSystem(SUT system);
        protected abstract void postSequenceProcessing();
        protected abstract void closeTestSession();
    }
}
