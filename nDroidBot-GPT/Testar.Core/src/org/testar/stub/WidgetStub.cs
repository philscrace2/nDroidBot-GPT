using System.Collections;
using System.Collections.Generic;
using System.Linq;
using org.testar.monkey;
using org.testar.monkey.alayer;

namespace org.testar.stub
{
    [Serializable]
    public class WidgetStub : TaggableBase, Widget
    {
        private readonly List<Widget> widgets = new();
        private Widget? parentWidget;

        public State root()
        {
            Widget current = this;
            while (current.parent() != null)
            {
                current = current.parent()!;
            }

            if (current is State state)
            {
                return state;
            }

            throw new InvalidOperationException("Widget tree root is not a State.");
        }

        public Widget? parent()
        {
            return parentWidget;
        }

        public void setParent(Widget parent)
        {
            parentWidget = parent;
        }

        public Widget child(int index)
        {
            return widgets[index];
        }

        public int childCount()
        {
            return widgets.Count;
        }

        public void remove()
        {
            if (parentWidget is WidgetStub parentStub)
            {
                parentStub.widgets.Remove(this);
            }

            parentWidget = null;
        }

        public void moveTo(Widget parent, int index)
        {
            if (parent is not WidgetStub newParent)
            {
                throw new ArgumentException("Parent must be a WidgetStub.", nameof(parent));
            }

            remove();
            parentWidget = newParent;
            index = Math.Clamp(index, 0, newParent.widgets.Count);
            newParent.widgets.Insert(index, this);
        }

        public Widget addChild()
        {
            var child = new WidgetStub();
            child.parentWidget = this;
            widgets.Add(child);
            return child;
        }

        public Widget addChild(Widget widget)
        {
            if (widget is WidgetStub child)
            {
                child.parentWidget = this;
            }

            widgets.Add(widget);
            return widget;
        }

        public Drag[]? scrollDrags(double scrollArrowSize, double scrollThick)
        {
            return null;
        }

        public string getRepresentation(string tab)
        {
            return tab + "WIDGET = " + get(Tags.ConcreteID, string.Empty) + ", " +
                   get(Tags.Abstract_R_ID, string.Empty) + ", " +
                   get(Tags.Abstract_R_T_ID, string.Empty) + ", " +
                   get(Tags.Abstract_R_T_P_ID, string.Empty) + "\n";
        }

        public string toString(params ITag[] tags)
        {
            if (tags.Length == 0)
            {
                return ToString() ?? string.Empty;
            }

            var parts = new List<string>();
            foreach (var tag in tags)
            {
                parts.Add($"{tag.name()}={getTagValue(tag)}");
            }

            return string.Join(" ", parts);
        }

        private object? getTagValue(ITag tag)
        {
            var method = GetType().GetMethods()
                .FirstOrDefault(m => m.Name == "get" && m.IsGenericMethod && m.GetParameters().Length == 2);
            if (method == null)
            {
                return null;
            }

            var generic = method.MakeGenericMethod(tag.type());
            return generic.Invoke(this, new object?[] { tag, null });
        }
    }
}
