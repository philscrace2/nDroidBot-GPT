using System;
using System.Text;
using org.testar.monkey;
using org.testar.monkey.alayer;
using org.testar.monkey.alayer.devices;

namespace org.testar.monkey.alayer.actions
{
    [Serializable]
    public sealed class Type : Action
    {
        private readonly string text;

        public Type(string text)
        {
            Assert.hasText(text);
            this.text = text;
            set(Tags.Role, ActionRoles.Type);
        }

        public override void run(SUT system, State state, double duration)
        {
            Assert.isTrue(duration >= 0);
            Assert.notNull(system);

            string inputText = get(Tags.InputText, text);
            if (inputText.Length == 0)
            {
                return;
            }

            double delay = duration / inputText.Length;
            var keyboard = system.get(Tags.StandardKeyboard);
            foreach (char c in inputText)
            {
                if (TryGetKey(c, out var key, out bool shift))
                {
                    if (shift)
                    {
                        keyboard.press(KBKeys.VK_SHIFT);
                    }

                    keyboard.press(key);
                    keyboard.release(key);

                    if (shift)
                    {
                        keyboard.release(KBKeys.VK_SHIFT);
                    }
                }
                else
                {
                    throw new ArgumentException($"Unable to find key for character '{c}'.");
                }

                Util.pause(delay);
            }
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

        public override string toString(params Role[] discardParameters)
        {
            foreach (var role in discardParameters)
            {
                if (role.name() == ActionRoles.Type.name())
                {
                    return "Text typed";
                }
            }

            return ToString();
        }

        public override string ToString()
        {
            return "Type text '" + get(Tags.InputText, text) + "'";
        }

        private static bool TryGetKey(char c, out KBKeys key, out bool shift)
        {
            shift = false;

            if (char.IsLetter(c))
            {
                shift = char.IsUpper(c);
                char upper = char.ToUpperInvariant(c);
                key = (KBKeys)Enum.Parse(typeof(KBKeys), $"VK_{upper}");
                return true;
            }

            if (char.IsDigit(c))
            {
                key = (KBKeys)Enum.Parse(typeof(KBKeys), $"VK_{c}");
                return true;
            }

            if (c == ' ')
            {
                key = KBKeys.VK_SPACE;
                return true;
            }

            key = default;
            return false;
        }
    }
}
