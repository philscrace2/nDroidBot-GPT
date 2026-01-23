using org.testar.monkey;

namespace org.testar.monkey.alayer
{
    public interface Widget : Taggable
    {
        State root();
        Widget? parent();
        Widget child(int index);
        int childCount();
        void remove();
        void moveTo(Widget parent, int index);
        Widget addChild();
        Drag[]? scrollDrags(double scrollArrowSize, double scrollThick);
        string getRepresentation(string tab);
        string toString(params ITag[] tags);
    }
}
