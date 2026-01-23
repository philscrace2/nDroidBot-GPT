using org.testar.monkey;
using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.actions
{
    [Serializable]
    public sealed class PasteText : Action
    {
        private readonly string text;

        public PasteText(string text)
        {
            Assert.hasText(text);
            this.text = text;
            set(Tags.Role, ActionRoles.Paste);
        }

        public override void run(SUT system, State state, double duration)
        {
            Assert.isTrue(duration >= 0);
            Assert.notNull(system);
        }

        public override string toString(params Role[] discardParameters)
        {
            foreach (var role in discardParameters)
            {
                if (role.name() == ActionRoles.Type.name())
                {
                    return "Text pasted";
                }
            }

            return ToString();
        }

        public override string toShortString()
        {
            Role? role = get(Tags.Role, default(Role));
            return role != null ? role.ToString() : ToString();
        }

        public override string toParametersString()
        {
            return "(" + get(Tags.InputText, text) + ")";
        }

        public override string ToString()
        {
            return "Pasted text '" + get(Tags.InputText, text) + "'";
        }
    }
}
