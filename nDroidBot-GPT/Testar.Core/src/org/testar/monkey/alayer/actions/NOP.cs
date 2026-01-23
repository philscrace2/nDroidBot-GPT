using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.actions
{
    [Serializable]
    public sealed class NOP : Action
    {
        public const string NOP_ID = "No Operation";

        public NOP()
        {
            set(Tags.Desc, NOP_ID);
            set(Tags.Role, ActionRoles.NOPAction);
        }

        public override void run(SUT system, State state, double duration)
        {
        }

        public override string toShortString()
        {
            Role? role = get(Tags.Role, default(Role));
            return role != null ? role.ToString() : ToString();
        }

        public override string toParametersString()
        {
            return string.Empty;
        }

        public override string toString(params Role[] discardParameters)
        {
            return ToString();
        }

        public override string ToString()
        {
            return NOP_ID;
        }
    }
}
