using System.Collections;
using System.Collections.Generic;
using org.testar.monkey.alayer;

namespace org.testar.stub
{
    [Serializable]
    public class StateStub : WidgetStub, State
    {
        public IEnumerator<Widget> GetEnumerator()
        {
            return new WidgetIterator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
