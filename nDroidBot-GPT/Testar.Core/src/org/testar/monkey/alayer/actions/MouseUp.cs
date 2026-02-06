using org.testar.monkey;
using org.testar.monkey.alayer;
using org.testar.monkey.alayer.devices;

namespace org.testar.monkey.alayer.actions
{
    [Serializable]
    public sealed class MouseUp : Action
    {
        private readonly MouseButtons button;

        public MouseUp(MouseButtons button)
        {
            this.button = button;
            set(Tags.Role, ActionRoles.MouseUp);
        }

        public override void run(SUT system, State state, double duration)
        {
            Assert.isTrue(duration >= 0);
            Assert.notNull(system);
            var mouse = system.get(Tags.StandardMouse);
            mouse.release(button);
        }

        public override string toShortString()
        {
            Role? role = get(Tags.Role, default(Role));
            return role != null ? role.ToString() : ToString();
        }

        public override string toParametersString()
        {
            return "(" + button + ")";
        }

        public override string toString(params Role[] discardParameters)
        {
            foreach (var role in discardParameters)
            {
                if (role.name() == ActionRoles.MouseUp.name())
                {
                    return "Mouse released";
                }
            }

            return ToString();
        }

        public override string ToString()
        {
            return $"Mouse Up {button}";
        }
    }
}
