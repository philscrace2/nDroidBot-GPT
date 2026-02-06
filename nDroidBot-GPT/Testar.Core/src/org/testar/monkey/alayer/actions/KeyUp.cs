using org.testar.monkey.alayer;
using org.testar.monkey.alayer.devices;

namespace org.testar.monkey.alayer.actions
{
    [Serializable]
    public sealed class KeyUp : KeyAction
    {
        public KeyUp(KBKeys key) : base(key)
        {
            set(Tags.Role, ActionRoles.KeyUp);
        }

        public override string ToString()
        {
            return $"Release Key {key}";
        }

        public override string toString(params Role[] discardParameters)
        {
            foreach (var role in discardParameters)
            {
                if (role.name() == ActionRoles.KeyUp.name())
                {
                    return "Key released";
                }
            }

            return ToString();
        }

        public override bool Equals(object? obj)
        {
            return obj is KeyUp other && key.Equals(other.key);
        }

        public override int GetHashCode()
        {
            return key.GetHashCode();
        }

        public override void run(SUT system, State state, double duration)
        {
            base.run(system, state, duration);
            var keyboard = system.get(Tags.StandardKeyboard);
            keyboard.release(key);
        }
    }
}
