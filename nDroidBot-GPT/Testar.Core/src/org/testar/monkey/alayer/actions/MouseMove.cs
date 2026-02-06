using org.testar.monkey;
using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.actions
{
    [Serializable]
    public sealed class MouseMove : Action
    {
        private readonly Position position;
        private readonly double minDuration;

        public MouseMove(Point point) : this(new AbsolutePosition(point), 0)
        {
        }

        public MouseMove(double x, double y) : this(new AbsolutePosition(x, y), 0)
        {
        }

        public MouseMove(Position position) : this(position, 0)
        {
        }

        public MouseMove(Position position, double minDuration)
        {
            Assert.notNull(position);
            Assert.isTrue(minDuration >= 0);
            this.position = position;
            this.minDuration = minDuration;
            set(Tags.Role, ActionRoles.MouseMove);
        }

        public override void run(SUT system, State state, double duration)
        {
            Assert.notNull(system, state);
            var point = position.apply(state);
            var mouse = system.get(Tags.StandardMouse);
            Util.moveCursor(mouse, point.x(), point.y(), Math.Max(duration, minDuration));
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
            foreach (var role in discardParameters)
            {
                if (role.name() == ActionRoles.MouseMove.name())
                {
                    return "Mouse moved";
                }
            }

            return ToString();
        }

        public override string ToString()
        {
            return $"Move mouse to {position}.";
        }
    }
}
