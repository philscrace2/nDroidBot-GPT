using org.testar.monkey.alayer;
using org.testar.stub;

namespace org.testar.monkey.alayer.windows
{
    public class UIAWidget : WidgetStub
    {
        public UIAWidget()
        {
        }

        public UIAWidget(string concreteId, string title, Rect bounds, Role role, bool enabled, string frameworkId, bool isModal, long hwnd)
        {
            set(Tags.ConcreteID, concreteId);
            set(Tags.AbstractID, concreteId);
            set(Tags.Abstract_R_ID, concreteId);
            set(Tags.Abstract_R_T_ID, concreteId);
            set(Tags.Abstract_R_T_P_ID, concreteId);
            set(Tags.Title, title);
            set(Tags.Desc, title);
            set(Tags.Shape, bounds);
            set(Tags.Role, role);
            set(Tags.Enabled, enabled);
            set(Tags.Blocked, false);
            set(Tags.HWND, hwnd);
            set(UIATags.UIAFrameworkId, frameworkId);
            set(UIATags.UIAIsWindowModal, isModal);
        }

        public UIAWidget addChild(UIAWidget child)
        {
            child.setParent(this);
            addChild((Widget)child);
            return child;
        }
    }
}
