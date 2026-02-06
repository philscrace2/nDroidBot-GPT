using System.Collections.Generic;
using org.testar.monkey.alayer;
using org.testar.settings;

namespace org.testar.monkey
{
    public class DefaultProtocol : RuntimeControlsProtocol
    {
        public DefaultProtocol(Settings settings) : base(settings)
        {
        }

        public void Run()
        {
            throw new System.NotImplementedException();
        }

        protected override void initialize(Settings settings)
        {
            throw new System.NotImplementedException();
        }

        protected override void initTestSession()
        {
            throw new System.NotImplementedException();
        }

        protected override void preSequencePreparations()
        {
            throw new System.NotImplementedException();
        }

        protected override SUT startSystem()
        {
            throw new System.NotImplementedException();
        }

        protected override void beginSequence(SUT system, State state)
        {
            throw new System.NotImplementedException();
        }

        protected override State getState(SUT system)
        {
            throw new System.NotImplementedException();
        }

        protected override Verdict getVerdict(State state)
        {
            throw new System.NotImplementedException();
        }

        protected override ISet<Action> deriveActions(SUT system, State state)
        {
            throw new System.NotImplementedException();
        }

        protected override Action selectAction(State state, ISet<Action> actions)
        {
            throw new System.NotImplementedException();
        }

        protected override bool executeAction(SUT system, State state, Action action)
        {
            throw new System.NotImplementedException();
        }

        protected override bool moreActions(State state)
        {
            throw new System.NotImplementedException();
        }

        protected override bool moreSequences()
        {
            throw new System.NotImplementedException();
        }

        protected override void finishSequence()
        {
            throw new System.NotImplementedException();
        }

        protected override void stopSystem(SUT system)
        {
            throw new System.NotImplementedException();
        }

        protected override void postSequenceProcessing()
        {
            throw new System.NotImplementedException();
        }

        protected override void closeTestSession()
        {
            throw new System.NotImplementedException();
        }
    }
}
