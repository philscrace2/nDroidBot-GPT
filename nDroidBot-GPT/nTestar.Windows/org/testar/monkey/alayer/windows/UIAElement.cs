using System.Windows.Automation;
using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.windows
{
    public class UIAElement
    {
        private readonly List<UIAElement> children = new();
        private readonly Dictionary<ITag, object?> extraTags = new();

        public string Name { get; }
        public string FrameworkId { get; }
        public string ControlType { get; }
        public long ControlTypeId { get; }
        public long NativeWindowHandle { get; }
        public long ProcessId { get; }
        public long Culture { get; }
        public long Orientation { get; }
        public bool IsEnabled { get; }
        public bool IsModal { get; }
        public bool IsContentElement { get; }
        public bool IsControlElement { get; }
        public bool HasKeyboardFocus { get; }
        public bool IsKeyboardFocusable { get; }
        public bool IsOffscreen { get; }
        public string HelpText { get; }
        public string ClassName { get; }
        public string AutomationId { get; }
        public string AcceleratorKey { get; }
        public string AccessKey { get; }
        public string ItemType { get; }
        public string ItemStatus { get; }
        public string ProviderDescription { get; }
        public string LocalizedControlType { get; }
        public string ValuePattern { get; }
        public long WindowInteractionState { get; }
        public long WindowVisualState { get; }
        public bool IsScrollPatternAvailable { get; }
        public bool IsTogglePatternAvailable { get; }
        public bool IsValuePatternAvailable { get; }
        public bool IsWindowPatternAvailable { get; }
        public bool IsSelectionPatternAvailable { get; }
        public bool IsSelectionItemPatternAvailable { get; }
        public bool HorizontallyScrollable { get; }
        public bool VerticallyScrollable { get; }
        public double ScrollHorizontalViewSize { get; }
        public double ScrollVerticalViewSize { get; }
        public double ScrollHorizontalPercent { get; }
        public double ScrollVerticalPercent { get; }
        public long ToggleState { get; }
        public bool ValueIsReadOnly { get; }
        public bool WindowCanMaximize { get; }
        public bool WindowCanMinimize { get; }
        public bool WindowIsTopmost { get; }
        public bool SelectionCanSelectMultiple { get; }
        public bool SelectionIsSelectionRequired { get; }
        public object? SelectionSelection { get; }
        public bool SelectionItemIsSelected { get; }
        public object? SelectionItemSelectionContainer { get; }
        public Rect Bounds { get; }
        public double ZIndex { get; private set; }
        public UIAElement? Parent { get; private set; }
        public IReadOnlyList<UIAElement> Children => children;

        public UIAElement(
            string name,
            string frameworkId,
            string controlType,
            long controlTypeId,
            long nativeWindowHandle,
            long processId,
            long culture,
            long orientation,
            bool isEnabled,
            bool isModal,
            bool isContentElement,
            bool isControlElement,
            bool hasKeyboardFocus,
            bool isKeyboardFocusable,
            bool isOffscreen,
            string helpText,
            string className,
            string automationId,
            string acceleratorKey,
            string accessKey,
            string itemType,
            string itemStatus,
            string providerDescription,
            string localizedControlType,
            string valuePattern,
            long windowInteractionState,
            long windowVisualState,
            Rect bounds,
            double zIndex = 0.0,
            bool isScrollPatternAvailable = false,
            bool isTogglePatternAvailable = false,
            bool isValuePatternAvailable = false,
            bool isWindowPatternAvailable = false,
            bool isSelectionPatternAvailable = false,
            bool isSelectionItemPatternAvailable = false,
            bool horizontallyScrollable = false,
            bool verticallyScrollable = false,
            double scrollHorizontalViewSize = 0.0,
            double scrollVerticalViewSize = 0.0,
            double scrollHorizontalPercent = -1.0,
            double scrollVerticalPercent = -1.0,
            long toggleState = 0,
            bool valueIsReadOnly = false,
            bool windowCanMaximize = true,
            bool windowCanMinimize = true,
            bool windowIsTopmost = false,
            bool selectionCanSelectMultiple = false,
            bool selectionIsSelectionRequired = false,
            object? selectionSelection = null,
            bool selectionItemIsSelected = false,
            object? selectionItemSelectionContainer = null)
        {
            Name = name;
            FrameworkId = frameworkId;
            ControlType = controlType;
            ControlTypeId = controlTypeId;
            NativeWindowHandle = nativeWindowHandle;
            ProcessId = processId;
            Culture = culture;
            Orientation = orientation;
            IsEnabled = isEnabled;
            IsModal = isModal;
            IsContentElement = isContentElement;
            IsControlElement = isControlElement;
            HasKeyboardFocus = hasKeyboardFocus;
            IsKeyboardFocusable = isKeyboardFocusable;
            IsOffscreen = isOffscreen;
            HelpText = helpText;
            ClassName = className;
            AutomationId = automationId;
            AcceleratorKey = acceleratorKey;
            AccessKey = accessKey;
            ItemType = itemType;
            ItemStatus = itemStatus;
            ProviderDescription = providerDescription;
            LocalizedControlType = localizedControlType;
            ValuePattern = valuePattern;
            WindowInteractionState = windowInteractionState;
            WindowVisualState = windowVisualState;
            IsScrollPatternAvailable = isScrollPatternAvailable;
            IsTogglePatternAvailable = isTogglePatternAvailable;
            IsValuePatternAvailable = isValuePatternAvailable;
            IsWindowPatternAvailable = isWindowPatternAvailable;
            IsSelectionPatternAvailable = isSelectionPatternAvailable;
            IsSelectionItemPatternAvailable = isSelectionItemPatternAvailable;
            HorizontallyScrollable = horizontallyScrollable;
            VerticallyScrollable = verticallyScrollable;
            ScrollHorizontalViewSize = scrollHorizontalViewSize;
            ScrollVerticalViewSize = scrollVerticalViewSize;
            ScrollHorizontalPercent = scrollHorizontalPercent;
            ScrollVerticalPercent = scrollVerticalPercent;
            ToggleState = toggleState;
            ValueIsReadOnly = valueIsReadOnly;
            WindowCanMaximize = windowCanMaximize;
            WindowCanMinimize = windowCanMinimize;
            WindowIsTopmost = windowIsTopmost;
            SelectionCanSelectMultiple = selectionCanSelectMultiple;
            SelectionIsSelectionRequired = selectionIsSelectionRequired;
            SelectionSelection = selectionSelection;
            SelectionItemIsSelected = selectionItemIsSelected;
            SelectionItemSelectionContainer = selectionItemSelectionContainer;
            Bounds = bounds;
            ZIndex = zIndex;
        }

        public void SetZIndex(double zIndex)
        {
            ZIndex = zIndex;
        }

        public void AddChild(UIAElement child)
        {
            child.Parent = this;
            children.Add(child);
        }

        public void SetExtraTag(ITag tag, object? value)
        {
            if (value != null)
            {
                extraTags[tag] = value;
            }
        }

        public bool TryGetExtraTag(ITag tag, out object? value)
        {
            return extraTags.TryGetValue(tag, out value);
        }

        public bool Contains(double x, double y)
        {
            return Bounds.contains(x, y);
        }

        public static UIAElement? TryFromAutomationElement(AutomationElement automationElement)
        {
            AutomationElement.AutomationElementInformation current;
            try
            {
                current = automationElement.Current;
            }
            catch
            {
                return null;
            }
            System.Windows.Rect rect = current.BoundingRectangle;
            if (rect.IsEmpty)
            {
                return null;
            }

            string name = current.Name ?? string.Empty;
            string frameworkId = current.FrameworkId ?? string.Empty;
            int hwnd = current.NativeWindowHandle;
            int processId = current.ProcessId;
            int culture = GetCurrentProperty(automationElement, AutomationElement.CultureProperty, 0);
            int orientation = (int)current.Orientation;
            bool isEnabled = current.IsEnabled;
            bool isModal = false;
            bool isContentElement = current.IsContentElement;
            bool isControlElement = current.IsControlElement;
            bool hasKeyboardFocus = current.HasKeyboardFocus;
            bool isKeyboardFocusable = current.IsKeyboardFocusable;
            bool isOffscreen = current.IsOffscreen;
            string helpText = current.HelpText ?? string.Empty;
            string className = current.ClassName ?? string.Empty;
            string automationId = current.AutomationId ?? string.Empty;
            string acceleratorKey = current.AcceleratorKey ?? string.Empty;
            string accessKey = current.AccessKey ?? string.Empty;
            string itemType = current.ItemType ?? string.Empty;
            string itemStatus = current.ItemStatus ?? string.Empty;
            string providerDescription = string.Empty;
            string localizedControlType = current.LocalizedControlType ?? string.Empty;
            long windowInteractionState = GetCurrentProperty(automationElement, WindowPattern.WindowInteractionStateProperty, 0L);
            long windowVisualState = GetCurrentProperty(automationElement, WindowPattern.WindowVisualStateProperty, 0L);

            ControlType? controlType = current.ControlType;
            string controlTypeName = controlType?.ProgrammaticName ?? "Control";
            int controlTypeId = controlType?.Id ?? 0;

            System.Windows.Automation.ValuePattern? valuePatternObj = TryGetCurrentPattern<System.Windows.Automation.ValuePattern>(automationElement, System.Windows.Automation.ValuePattern.Pattern);
            string valuePattern = string.Empty;
            bool valueIsReadOnly = false;
            if (valuePatternObj != null)
            {
                System.Windows.Automation.ValuePattern.ValuePatternInformation valueCurrent = valuePatternObj.Current;
                valuePattern = valueCurrent.Value ?? string.Empty;
                valueIsReadOnly = valueCurrent.IsReadOnly;
            }

            ScrollPattern? scrollPatternObj = TryGetCurrentPattern<ScrollPattern>(automationElement, ScrollPattern.Pattern);
            bool horizontallyScrollable = false;
            bool verticallyScrollable = false;
            double scrollHorizontalViewSize = 0.0;
            double scrollVerticalViewSize = 0.0;
            double scrollHorizontalPercent = -1.0;
            double scrollVerticalPercent = -1.0;
            if (scrollPatternObj != null)
            {
                ScrollPattern.ScrollPatternInformation scrollCurrent = scrollPatternObj.Current;
                horizontallyScrollable = scrollCurrent.HorizontallyScrollable;
                verticallyScrollable = scrollCurrent.VerticallyScrollable;
                scrollHorizontalViewSize = scrollCurrent.HorizontalViewSize;
                scrollVerticalViewSize = scrollCurrent.VerticalViewSize;
                scrollHorizontalPercent = scrollCurrent.HorizontalScrollPercent;
                scrollVerticalPercent = scrollCurrent.VerticalScrollPercent;
            }

            TogglePattern? togglePatternObj = TryGetCurrentPattern<TogglePattern>(automationElement, TogglePattern.Pattern);
            long toggleState = togglePatternObj == null ? 0 : (long)togglePatternObj.Current.ToggleState;

            WindowPattern? windowPatternObj = TryGetCurrentPattern<WindowPattern>(automationElement, WindowPattern.Pattern);
            bool windowCanMaximize = true;
            bool windowCanMinimize = true;
            bool windowIsTopmost = false;
            long windowInteraction = windowInteractionState;
            long windowVisual = windowVisualState;
            if (windowPatternObj != null)
            {
                WindowPattern.WindowPatternInformation windowCurrent = windowPatternObj.Current;
                windowCanMaximize = windowCurrent.CanMaximize;
                windowCanMinimize = windowCurrent.CanMinimize;
                isModal = windowCurrent.IsModal;
                windowIsTopmost = windowCurrent.IsTopmost;
                windowInteraction = (long)windowCurrent.WindowInteractionState;
                windowVisual = (long)windowCurrent.WindowVisualState;
            }

            SelectionPattern? selectionPatternObj = TryGetCurrentPattern<SelectionPattern>(automationElement, SelectionPattern.Pattern);
            bool selectionCanSelectMultiple = false;
            bool selectionIsSelectionRequired = false;
            object? selectionSelection = null;
            if (selectionPatternObj != null)
            {
                SelectionPattern.SelectionPatternInformation selectionCurrent = selectionPatternObj.Current;
                selectionCanSelectMultiple = selectionCurrent.CanSelectMultiple;
                selectionIsSelectionRequired = selectionCurrent.IsSelectionRequired;
                selectionSelection = selectionPatternObj.Current.GetSelection();
            }

            SelectionItemPattern? selectionItemPatternObj = TryGetCurrentPattern<SelectionItemPattern>(automationElement, SelectionItemPattern.Pattern);
            bool selectionItemIsSelected = false;
            object? selectionItemSelectionContainer = null;
            if (selectionItemPatternObj != null)
            {
                SelectionItemPattern.SelectionItemPatternInformation selectionItemCurrent = selectionItemPatternObj.Current;
                selectionItemIsSelected = selectionItemCurrent.IsSelected;
                selectionItemSelectionContainer = selectionItemCurrent.SelectionContainer;
            }

            return new UIAElement(
                name,
                frameworkId,
                controlTypeName,
                controlTypeId,
                hwnd,
                processId,
                culture,
                orientation,
                isEnabled,
                isModal,
                isContentElement,
                isControlElement,
                hasKeyboardFocus,
                isKeyboardFocusable,
                isOffscreen,
                helpText,
                className,
                automationId,
                acceleratorKey,
                accessKey,
                itemType,
                itemStatus,
                providerDescription,
                localizedControlType,
                valuePattern,
                windowInteraction,
                windowVisual,
                Rect.from(rect.X, rect.Y, Math.Max(1, rect.Width), Math.Max(1, rect.Height)),
                isScrollPatternAvailable: scrollPatternObj != null,
                isTogglePatternAvailable: togglePatternObj != null,
                isValuePatternAvailable: valuePatternObj != null,
                isWindowPatternAvailable: windowPatternObj != null,
                isSelectionPatternAvailable: selectionPatternObj != null,
                isSelectionItemPatternAvailable: selectionItemPatternObj != null,
                horizontallyScrollable: horizontallyScrollable,
                verticallyScrollable: verticallyScrollable,
                scrollHorizontalViewSize: scrollHorizontalViewSize,
                scrollVerticalViewSize: scrollVerticalViewSize,
                scrollHorizontalPercent: scrollHorizontalPercent,
                scrollVerticalPercent: scrollVerticalPercent,
                toggleState: toggleState,
                valueIsReadOnly: valueIsReadOnly,
                windowCanMaximize: windowCanMaximize,
                windowCanMinimize: windowCanMinimize,
                windowIsTopmost: windowIsTopmost,
                selectionCanSelectMultiple: selectionCanSelectMultiple,
                selectionIsSelectionRequired: selectionIsSelectionRequired,
                selectionSelection: selectionSelection,
                selectionItemIsSelected: selectionItemIsSelected,
                selectionItemSelectionContainer: selectionItemSelectionContainer);
        }

        private static TPattern? TryGetCurrentPattern<TPattern>(AutomationElement automationElement, AutomationPattern pattern)
            where TPattern : class
        {
            try
            {
                return automationElement.TryGetCurrentPattern(pattern, out object patternObject)
                    ? patternObject as TPattern
                    : null;
            }
            catch
            {
                return null;
            }
        }

        private static T GetCurrentProperty<T>(AutomationElement automationElement, AutomationProperty property, T defaultValue)
        {
            object? value;
            try
            {
                value = automationElement.GetCurrentPropertyValue(property, false);
            }
            catch
            {
                return defaultValue;
            }

            if (value == null || ReferenceEquals(AutomationElement.NotSupported, value))
            {
                return defaultValue;
            }

            if (value is T typed)
            {
                return typed;
            }

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
