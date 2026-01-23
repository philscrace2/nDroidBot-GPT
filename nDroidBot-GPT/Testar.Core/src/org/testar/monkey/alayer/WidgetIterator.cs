using System.Collections;
using System.Collections.Generic;
using org.testar.monkey;

namespace org.testar.monkey.alayer
{
    public sealed class WidgetIterator : IEnumerator<Widget>
    {
        private readonly Queue<Widget> buffer = new();
        private Widget? current;

        public WidgetIterator(Widget start)
        {
            Assert.notNull(start);
            buffer.Enqueue(start);
        }

        public Widget Current => current ?? throw new InvalidOperationException();
        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if (buffer.Count == 0)
            {
                return false;
            }

            current = buffer.Dequeue();
            for (int i = 0; i < current.childCount(); i++)
            {
                buffer.Enqueue(current.child(i));
            }

            return true;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
        }
    }
}
