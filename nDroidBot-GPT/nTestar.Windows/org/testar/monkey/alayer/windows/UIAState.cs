using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.windows
{
    public class UIAState : State
    {
        public State root() => throw new System.NotImplementedException();
        public Widget? parent() => throw new System.NotImplementedException();
        public Widget child(int index) => throw new System.NotImplementedException();
        public int childCount() => throw new System.NotImplementedException();
        public void remove() => throw new System.NotImplementedException();
        public void moveTo(Widget parent, int index) => throw new System.NotImplementedException();
        public Widget addChild() => throw new System.NotImplementedException();
        public Drag[]? scrollDrags(double scrollArrowSize, double scrollThick) => throw new System.NotImplementedException();
        public string getRepresentation(string tab) => throw new System.NotImplementedException();
        public string toString(params ITag[] tags) => throw new System.NotImplementedException();
        public T get<T>(Tag<T> tag) => throw new System.NotImplementedException();
        public T get<T>(Tag<T> tag, T defaultValue) => throw new System.NotImplementedException();
        public void set<T>(Tag<T> tag, T value) => throw new System.NotImplementedException();
        public void remove(ITag tag) => throw new System.NotImplementedException();
        public System.Collections.Generic.IEnumerable<ITag> tags() => throw new System.NotImplementedException();
        public System.Collections.IEnumerator GetEnumerator() => ((System.Collections.Generic.IEnumerable<Widget>)this).GetEnumerator();
    }
}
