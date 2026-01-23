using System.Collections.Generic;
using org.testar.monkey.alayer;
using org.testar.monkey.alayer.devices;

namespace org.testar.monkey.alayer.actions
{
    public sealed class StdActionCompiler
    {
        private readonly Action nop = new NOP();

        public Action hitKey(KBKeys key)
        {
            return new CompoundAction.Builder()
                .add(new KeyDown(key), 0)
                .add(new KeyUp(key), 1)
                .add(nop, 1)
                .build();
        }

        public Action hitShortcutKey(List<KBKeys> keys)
        {
            if (keys.Count == 1)
            {
                return hitKey(keys[0]);
            }

            var builder = new CompoundAction.Builder();
            for (int i = 0; i < keys.Count; i++)
            {
                builder.add(new KeyDown(keys[i]), i == 0 ? 0 : 0.1);
            }

            for (int i = keys.Count - 1; i >= 0; i--)
            {
                builder.add(new KeyUp(keys[i]), i == keys.Count - 1 ? 1.0 : 0);
            }

            builder.add(nop, 1.0);
            return builder.build();
        }
    }
}
