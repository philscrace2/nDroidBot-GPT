using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.windows
{
    public sealed class UIARootElement : UIAElement
    {
        public long Pid { get; init; }
        public bool IsRunning { get; init; } = true;
        public long TimeStamp { get; init; }
        public bool IsForeground { get; init; } = true;
        public bool HasStandardKeyboard { get; init; } = true;
        public bool HasStandardMouse { get; init; } = true;
        public ElementMap ElementMap { get; private set; } = ElementMap.NewBuilder().Build();

        public UIARootElement(
            long pid,
            long timeStamp,
            Rect bounds,
            bool isRunning = true,
            bool isForeground = true,
            bool hasStandardKeyboard = true,
            bool hasStandardMouse = true)
            : base(
                name: "Desktop",
                frameworkId: "UIA",
                controlType: "Root",
                controlTypeId: 0,
                nativeWindowHandle: 0,
                processId: pid,
                culture: 0,
                orientation: 0,
                isEnabled: true,
                isModal: false,
                isContentElement: true,
                isControlElement: true,
                hasKeyboardFocus: false,
                isKeyboardFocusable: false,
                isOffscreen: false,
                helpText: string.Empty,
                className: string.Empty,
                automationId: string.Empty,
                acceleratorKey: string.Empty,
                accessKey: string.Empty,
                itemType: string.Empty,
                itemStatus: string.Empty,
                providerDescription: string.Empty,
                localizedControlType: "desktop",
                valuePattern: string.Empty,
                windowInteractionState: 0,
                windowVisualState: 0,
                bounds: bounds)
        {
            Pid = pid;
            TimeStamp = timeStamp;
            IsRunning = isRunning;
            IsForeground = isForeground;
            HasStandardKeyboard = hasStandardKeyboard;
            HasStandardMouse = hasStandardMouse;
        }

        public UIAElement? At(double x, double y)
        {
            return ElementMap.At(x, y);
        }

        public bool VisibleAt(UIAElement element, double x, double y, bool obscuredByChildFeature = true)
        {
            if (!element.Contains(x, y) || !Contains(x, y))
            {
                return false;
            }

            UIAElement? topLevelContainer = ElementMap.At(x, y);
            return topLevelContainer == null
                   || topLevelContainer.ZIndex <= element.ZIndex
                   || !obscuredByChildFeature
                   || !ObscuredByChildren(element, x, y);
        }

        public void UpdateElementMap(ElementMap elementMap)
        {
            ElementMap = elementMap;
        }

        private static bool ObscuredByChildren(UIAElement element, double x, double y)
        {
            foreach (UIAElement child in element.Children)
            {
                if (child.ZIndex >= element.ZIndex && child.Contains(x, y))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
