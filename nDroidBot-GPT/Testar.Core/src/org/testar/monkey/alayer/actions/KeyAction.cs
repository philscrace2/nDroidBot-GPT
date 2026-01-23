using org.testar.monkey;
using org.testar.monkey.alayer;
using org.testar.monkey.alayer.devices;

namespace org.testar.monkey.alayer.actions
{
    [Serializable]
    public abstract class KeyAction : Action
    {
        protected readonly KBKeys key;

        protected KeyAction(KBKeys key)
        {
            this.key = key;
        }

        public override void run(SUT system, State state, double duration)
        {
            Assert.notNull(system);
        }

        public override string toShortString()
        {
            Role? role = get(Tags.Role, default(Role));
            return role != null ? role.ToString() : ToString();
        }

        public override string toParametersString()
        {
            return "(" + key + ")";
        }

        public abstract override string ToString();
    }
}
