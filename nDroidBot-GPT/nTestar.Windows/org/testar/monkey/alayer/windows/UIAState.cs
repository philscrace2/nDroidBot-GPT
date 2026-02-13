using org.testar.monkey.alayer;
using org.testar.stub;

namespace org.testar.monkey.alayer.windows
{
    public sealed class UIAState : StateStub
    {
        public UIAWidget addChild(UIAWidget child)
        {
            child.setParent(this);
            addChild((Widget)child);
            return child;
        }
    }
}
