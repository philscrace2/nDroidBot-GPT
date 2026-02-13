using org.testar.monkey.alayer;
using org.testar.stub;

namespace org.testar.monkey.alayer.windows
{
    public class UIAWidget : WidgetStub
    {
        private static readonly IReadOnlyCollection<ITag> FetchableTags = new ITag[]
        {
            Tags.Desc,
            Tags.Role,
            Tags.HitTester,
            Tags.Shape,
            Tags.Blocked,
            Tags.Enabled,
            Tags.Title,
            Tags.HWND,
            Tags.ZIndex,
            UIATags.UIAFrameworkId,
            UIATags.UIAIsWindowModal,
            UIATags.UIAName,
            UIATags.UIAControlType,
            UIATags.UIANativeWindowHandle,
            UIATags.UIAProcessId,
            UIATags.UIABoundingRectangle,
            UIATags.UIAIsEnabled,
            UIATags.UIAIsContentElement,
            UIATags.UIAIsControlElement,
            UIATags.UIAHasKeyboardFocus,
            UIATags.UIAIsKeyboardFocusable,
            UIATags.UIAIsOffscreen,
            UIATags.UIAHelpText,
            UIATags.UIAClassName,
            UIATags.UIAAutomationId,
            UIATags.UIAAcceleratorKey,
            UIATags.UIAAccessKey,
            UIATags.UIAItemType,
            UIATags.UIAItemStatus,
            UIATags.UIAProviderDescription,
            UIATags.UIALocalizedControlType,
            UIATags.UIAWindowInteractionState,
            UIATags.UIAWindowVisualState,
            UIATags.UIAIsScrollPatternAvailable,
            UIATags.UIAIsTogglePatternAvailable,
            UIATags.UIAIsValuePatternAvailable,
            UIATags.UIAIsWindowPatternAvailable,
            UIATags.UIAIsSelectionPatternAvailable,
            UIATags.UIAIsSelectionItemPatternAvailable,
            UIATags.UIAHorizontallyScrollable,
            UIATags.UIAVerticallyScrollable,
            UIATags.UIAScrollHorizontalViewSize,
            UIATags.UIAScrollVerticalViewSize,
            UIATags.UIAScrollHorizontalPercent,
            UIATags.UIAScrollVerticalPercent,
            UIATags.UIAToggleToggleState,
            UIATags.UIAValueIsReadOnly,
            UIATags.UIAValueValue,
            UIATags.UIAWindowCanMaximize,
            UIATags.UIAWindowCanMinimize,
            UIATags.UIAWindowIsModal,
            UIATags.UIAWindowIsTopmost,
            UIATags.UIAWindowWindowInteractionState,
            UIATags.UIAWindowWindowVisualState,
            UIATags.UIASelectionCanSelectMultiple,
            UIATags.UIASelectionIsSelectionRequired,
            UIATags.UIASelectionSelection,
            UIATags.UIASelectionItemIsSelected,
            UIATags.UIASelectionItemSelectionContainer
        };

        public UIAElement? AutomationElement { get; }

        public UIAWidget()
        {
        }

        public UIAWidget(UIAElement automationElement, string concreteId)
        {
            AutomationElement = automationElement;
            set(Tags.ConcreteID, concreteId);
            set(Tags.AbstractID, concreteId);
            set(Tags.Abstract_R_ID, concreteId);
            set(Tags.Abstract_R_T_ID, concreteId);
            set(Tags.Abstract_R_T_P_ID, concreteId);
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

        protected override T fetch<T>(Tag<T> tag)
        {
            Tag<T> resolvedTag = UIAMapping.getMappedStateTag(tag) ?? tag;
            object? resolved = ResolveTagValue(resolvedTag);
            if (resolved is T typed)
            {
                return typed;
            }

            return default!;
        }

        protected override IEnumerable<ITag> tagDomain()
        {
            return FetchableTags.Concat(UIATags.getAllActiveTags());
        }

        private object? ResolveTagValue(ITag tag)
        {
            UIAElement? element = AutomationElement;
            if (element == null)
            {
                return null;
            }

            if (tag == Tags.Desc || tag == Tags.Title)
            {
                return element.Name;
            }

            if (tag == Tags.Role)
            {
                return UIARoles.FromControlType(element.ControlType);
            }

            if (tag == Tags.Shape)
            {
                return element.Bounds;
            }

            if (tag == Tags.Blocked)
            {
                return false;
            }

            if (tag == Tags.Enabled)
            {
                return element.IsEnabled;
            }

            if (tag == Tags.HitTester)
            {
                return new UIAHitTester(this);
            }

            if (tag == Tags.HWND)
            {
                return element.NativeWindowHandle;
            }

            if (tag == Tags.ZIndex)
            {
                return element.ZIndex;
            }

            if (tag == UIATags.UIAFrameworkId)
            {
                return element.FrameworkId;
            }

            if (tag == UIATags.UIAName)
            {
                return element.Name;
            }

            if (tag == UIATags.UIAControlType)
            {
                return element.ControlTypeId;
            }

            if (tag == UIATags.UIANativeWindowHandle)
            {
                return element.NativeWindowHandle;
            }

            if (tag == UIATags.UIAProcessId)
            {
                return element.ProcessId;
            }

            if (tag == UIATags.UIABoundingRectangle)
            {
                return element.Bounds;
            }

            if (tag == UIATags.UIAIsEnabled)
            {
                return element.IsEnabled;
            }

            if (tag == UIATags.UIAIsContentElement)
            {
                return element.IsContentElement;
            }

            if (tag == UIATags.UIAIsControlElement)
            {
                return element.IsControlElement;
            }

            if (tag == UIATags.UIAHasKeyboardFocus)
            {
                return element.HasKeyboardFocus;
            }

            if (tag == UIATags.UIAIsKeyboardFocusable)
            {
                return element.IsKeyboardFocusable;
            }

            if (tag == UIATags.UIAIsOffscreen)
            {
                return element.IsOffscreen;
            }

            if (tag == UIATags.UIAHelpText)
            {
                return element.HelpText;
            }

            if (tag == UIATags.UIAClassName)
            {
                return element.ClassName;
            }

            if (tag == UIATags.UIAAutomationId)
            {
                return element.AutomationId;
            }

            if (tag == UIATags.UIAAcceleratorKey)
            {
                return element.AcceleratorKey;
            }

            if (tag == UIATags.UIAAccessKey)
            {
                return element.AccessKey;
            }

            if (tag == UIATags.UIAItemType)
            {
                return element.ItemType;
            }

            if (tag == UIATags.UIAItemStatus)
            {
                return element.ItemStatus;
            }

            if (tag == UIATags.UIAProviderDescription)
            {
                return element.ProviderDescription;
            }

            if (tag == UIATags.UIALocalizedControlType)
            {
                return element.LocalizedControlType;
            }

            if (tag == UIATags.UIAIsWindowModal)
            {
                return element.IsModal;
            }

            if (tag == UIATags.UIAWindowInteractionState || tag == UIATags.UIAWindowWindowInteractionState)
            {
                return element.WindowInteractionState;
            }

            if (tag == UIATags.UIAWindowVisualState || tag == UIATags.UIAWindowWindowVisualState)
            {
                return element.WindowVisualState;
            }

            if (tag == UIATags.UIAIsScrollPatternAvailable)
            {
                return false;
            }

            if (tag == UIATags.UIAIsTogglePatternAvailable)
            {
                return false;
            }

            if (tag == UIATags.UIAIsValuePatternAvailable)
            {
                return !string.IsNullOrEmpty(element.ValuePattern);
            }

            if (tag == UIATags.UIAIsWindowPatternAvailable)
            {
                return element.ControlType.Contains("Window", StringComparison.OrdinalIgnoreCase);
            }

            if (tag == UIATags.UIAIsSelectionPatternAvailable || tag == UIATags.UIAIsSelectionItemPatternAvailable)
            {
                return false;
            }

            if (tag == UIATags.UIAHorizontallyScrollable || tag == UIATags.UIAVerticallyScrollable)
            {
                return false;
            }

            if (tag == UIATags.UIAScrollHorizontalViewSize || tag == UIATags.UIAScrollVerticalViewSize)
            {
                return 0.0;
            }

            if (tag == UIATags.UIAScrollHorizontalPercent || tag == UIATags.UIAScrollVerticalPercent)
            {
                return -1.0;
            }

            if (tag == UIATags.UIAToggleToggleState)
            {
                return 0L;
            }

            if (tag == UIATags.UIAValueIsReadOnly)
            {
                return false;
            }

            if (tag == UIATags.UIAValueValue)
            {
                return element.ValuePattern;
            }

            if (tag == UIATags.UIAWindowCanMaximize || tag == UIATags.UIAWindowCanMinimize)
            {
                return true;
            }

            if (tag == UIATags.UIAWindowIsModal)
            {
                return element.IsModal;
            }

            if (tag == UIATags.UIAWindowIsTopmost)
            {
                return false;
            }

            if (tag == UIATags.UIASelectionCanSelectMultiple || tag == UIATags.UIASelectionIsSelectionRequired)
            {
                return false;
            }

            if (tag == UIATags.UIASelectionSelection || tag == UIATags.UIASelectionItemSelectionContainer)
            {
                return null;
            }

            if (tag == UIATags.UIASelectionItemIsSelected)
            {
                return false;
            }

            return null;
        }
    }
}
