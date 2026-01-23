using System.Collections.ObjectModel;
using System.Linq;
using org.testar.monkey;
using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.actions
{
    [Serializable]
    public sealed class CompoundAction : Action
    {
        private readonly List<Action> actions;

        public sealed class Builder
        {
            private readonly List<Action> actions = new();

            public Builder add(Action action, double relativeDuration)
            {
                Assert.notNull(action);
                actions.Add(action);
                return this;
            }

            public CompoundAction build()
            {
                Assert.isTrue(actions.Count > 0, "Sum of durations needs to be larger than 0!");
                return new CompoundAction(actions);
            }
        }

        public CompoundAction(params Action[] actions)
        {
            this.actions = new List<Action>(actions ?? Array.Empty<Action>());
            set(Tags.Role, ActionRoles.CompoundAction);
        }

        private CompoundAction(IEnumerable<Action> actions)
        {
            this.actions = new List<Action>(actions);
            set(Tags.Role, ActionRoles.CompoundAction);
        }

        public IReadOnlyList<Action> getActions()
        {
            return new ReadOnlyCollection<Action>(actions);
        }

        public override void run(SUT system, State state, double duration)
        {
            foreach (Action action in actions)
            {
                action.run(system, state, duration);
            }
        }

        public override string toShortString()
        {
            return "CompoundAction";
        }

        public override string toParametersString()
        {
            return string.Concat(actions.Select(a => a.toParametersString()));
        }

        public override string toString(params Role[] discardParameters)
        {
            var lines = new List<string> { "Compound Action =" };
            lines.AddRange(actions.Select(a => a.toString(discardParameters)));
            return string.Join(Util.lineSep, lines);
        }

        public override string ToString()
        {
            var lines = new List<string> { "Compound Action =" };
            lines.AddRange(actions.Select(a => a.ToString()));
            return string.Join(Util.lineSep, lines);
        }
    }
}
