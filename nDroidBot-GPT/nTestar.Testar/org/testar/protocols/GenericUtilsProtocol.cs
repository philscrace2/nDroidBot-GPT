using System.Collections.Generic;
using org.testar.monkey;
using org.testar.monkey.alayer;
using org.testar.monkey.alayer.actions;
using org.testar.settings;
using Action = org.testar.monkey.alayer.Action;

namespace org.testar.protocols
{
    public class GenericUtilsProtocol : org.testar.monkey.DefaultProtocol
    {
        protected bool visualizationOn;
        protected Canvas cv = null!;
        protected org.testar.monkey.alayer.devices.Mouse mouse = null!;
        protected int sequenceCount;

        protected GenericUtilsProtocol() : base(new Settings())
        {
        }

        protected virtual void initialize(Settings settings)
        {
            this.settings = settings;
        }

        protected virtual void preSequencePreparations()
        {
        }

        protected virtual SUT startSystem()
        {
            throw new System.NotImplementedException();
        }

        protected virtual void beginSequence(SUT system, State state)
        {
        }

        protected virtual State getState(SUT system)
        {
            throw new System.NotImplementedException();
        }

        protected virtual Verdict getVerdict(State state)
        {
            return Verdict.OK;
        }

        protected virtual ISet<Action> deriveActions(SUT system, State state)
        {
            return new HashSet<Action>();
        }

        protected virtual Action selectAction(State state, ISet<Action> actions)
        {
            return new NOP();
        }

        protected virtual bool executeAction(SUT system, State state, Action action)
        {
            return true;
        }

        protected virtual bool moreActions(State state)
        {
            return true;
        }

        protected virtual bool moreSequences()
        {
            return true;
        }

        protected virtual void finishSequence()
        {
        }

        protected virtual void stopSystem(SUT system)
        {
        }

        protected virtual void postSequenceProcessing()
        {
        }

        protected virtual void buildStateIdentifiers(State state)
        {
        }

        protected virtual void buildStateActionsIdentifiers(State state, ISet<Action> actions)
        {
        }

        protected virtual void buildEnvironmentActionIdentifiers(State state, Action action)
        {
        }

        protected virtual ISet<Action> preSelectAction(SUT system, State state, ISet<Action> actions)
        {
            return actions;
        }

        protected virtual bool isTypeable(Widget widget)
        {
            return false;
        }

        protected virtual bool isAtBrowserCanvas(Widget widget)
        {
            return false;
        }

        protected org.testar.monkey.RuntimeControlsProtocol.Modes mode()
        {
            return org.testar.monkey.RuntimeControlsProtocol.Modes.Generate;
        }

        protected Settings settings()
        {
            return settings;
        }
    }
}
