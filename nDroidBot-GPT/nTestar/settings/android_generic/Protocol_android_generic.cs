using System.Collections.Generic;
using org.testar;
using org.testar.monkey;
using org.testar.monkey.alayer;
using org.testar.protocols;
using org.testar.settings;
using Action = org.testar.monkey.alayer.Action;
using State = org.testar.monkey.alayer.State;
using SUT = org.testar.monkey.alayer.SUT;
using Verdict = org.testar.monkey.alayer.Verdict;

public class Protocol_android_generic : AndroidProtocol
{
    protected override void initialize(Settings settings)
    {
        base.initialize(settings);
    }

    protected override void preSequencePreparations()
    {
        base.preSequencePreparations();
    }

    protected override SUT startSystem()
    {
        return base.startSystem();
    }

    protected override void beginSequence(SUT system, State state)
    {
        base.beginSequence(system, state);
    }

    protected override State getState(SUT system)
    {
        return base.getState(system);
    }

    protected override Verdict getVerdict(State state)
    {
        Verdict verdict = base.getVerdict(state);
        return verdict;
    }

    protected override ISet<Action> deriveActions(SUT system, State state)
    {
        ISet<Action> actions = base.deriveActions(system, state);
        return actions;
    }

    protected override Action selectAction(State state, ISet<Action> actions)
    {
        return base.selectAction(state, actions);
    }

    protected override bool executeAction(SUT system, State state, Action action)
    {
        return base.executeAction(system, state, action);
    }

    protected override bool moreActions(State state)
    {
        return base.moreActions(state);
    }

    protected override bool moreSequences()
    {
        return base.moreSequences();
    }

    protected override void finishSequence()
    {
        base.finishSequence();
    }

    protected override void stopSystem(SUT system)
    {
        base.stopSystem(system);
    }

    protected override void postSequenceProcessing()
    {
        base.postSequenceProcessing();
    }

    protected override void buildStateIdentifiers(State state)
    {
        base.buildStateIdentifiers(state);
    }

    protected override void buildStateActionsIdentifiers(State state, ISet<Action> actions)
    {
        base.buildStateActionsIdentifiers(state, actions);
    }

    protected override void buildEnvironmentActionIdentifiers(State state, Action action)
    {
        base.buildEnvironmentActionIdentifiers(state, action);
    }
}
