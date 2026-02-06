using org.testar.monkey.alayer;
using org.testar.monkey.alayer.devices;

namespace org.testar.monkey.alayer.actions
{
    [Serializable]
    public sealed class KeyDown : KeyAction
    {
        public KeyDown(KBKeys key) : base(key)
        {
            set(Tags.Role, ActionRoles.KeyDown);
        }

        public override string ToString()
        {
            return $"Press Key {key}";
        }

        public override string toString(params Role[] discardParameters)
        {
            foreach (var role in discardParameters)
            {
                if (role.name() == ActionRoles.KeyDown.name())
                {
                    return "Key pressed";
                }
            }

            return ToString();
        }

        public override bool Equals(object? obj)
        {
            return obj is KeyDown other && key.Equals(other.key);
        }

        public override int GetHashCode()
        {
            return key.GetHashCode();
        }

        public override void run(SUT system, State state, double duration)
        {
            base.run(system, state, duration);
            var keyboard = system.get(Tags.StandardKeyboard);
            keyboard.press(key);
        }
    }
}
