using System.Reflection;
using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.windows
{
    public class UIAElement
    {
        private readonly List<UIAElement> children = new();

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

        public bool Contains(double x, double y)
        {
            return Bounds.contains(x, y);
        }

        public static UIAElement? TryFromAutomationElement(object automationElement)
        {
            object? current = automationElement.GetType()
                .GetProperty("Current", BindingFlags.Public | BindingFlags.Instance)?
                .GetValue(automationElement);
            if (current == null)
            {
                return null;
            }

            object? rectObj = ReadOrDefault<object?>(current, "BoundingRectangle", null);
            if (rectObj == null)
            {
                return null;
            }

            double x = ReadOrDefault(rectObj, "X", 0.0);
            double y = ReadOrDefault(rectObj, "Y", 0.0);
            double width = ReadOrDefault(rectObj, "Width", 0.0);
            double height = ReadOrDefault(rectObj, "Height", 0.0);

            string name = ReadOrDefault(current, "Name", string.Empty);
            string frameworkId = ReadOrDefault(current, "FrameworkId", string.Empty);
            int hwnd = ReadOrDefault(current, "NativeWindowHandle", 0);
            int processId = ReadOrDefault(current, "ProcessId", 0);
            int culture = ReadOrDefault(current, "Culture", 0);
            int orientation = ReadOrDefault(current, "Orientation", 0);
            bool isEnabled = ReadOrDefault(current, "IsEnabled", true);
            bool isModal = ReadOrDefault(current, "IsModal", false);
            bool isContentElement = ReadOrDefault(current, "IsContentElement", false);
            bool isControlElement = ReadOrDefault(current, "IsControlElement", false);
            bool hasKeyboardFocus = ReadOrDefault(current, "HasKeyboardFocus", false);
            bool isKeyboardFocusable = ReadOrDefault(current, "IsKeyboardFocusable", false);
            bool isOffscreen = ReadOrDefault(current, "IsOffscreen", false);
            string helpText = ReadOrDefault(current, "HelpText", string.Empty);
            string className = ReadOrDefault(current, "ClassName", string.Empty);
            string automationId = ReadOrDefault(current, "AutomationId", string.Empty);
            string acceleratorKey = ReadOrDefault(current, "AcceleratorKey", string.Empty);
            string accessKey = ReadOrDefault(current, "AccessKey", string.Empty);
            string itemType = ReadOrDefault(current, "ItemType", string.Empty);
            string itemStatus = ReadOrDefault(current, "ItemStatus", string.Empty);
            string providerDescription = ReadOrDefault(current, "ProviderDescription", string.Empty);
            string localizedControlType = ReadOrDefault(current, "LocalizedControlType", string.Empty);
            int windowInteractionState = ReadOrDefault(current, "WindowInteractionState", 0);
            int windowVisualState = ReadOrDefault(current, "WindowVisualState", 0);

            object? controlType = ReadOrDefault<object?>(current, "ControlType", null);
            string controlTypeName = controlType?.GetType()
                                         .GetProperty("ProgrammaticName", BindingFlags.Public | BindingFlags.Instance)?
                                         .GetValue(controlType)?
                                         .ToString()
                                     ?? "Control";
            int controlTypeId = 0;
            if (controlType != null)
            {
                controlTypeId = ReadOrDefault(controlType, "Id", 0);
            }

            object? valuePatternObj = TryGetCurrentPatternObject(automationElement, "ValuePattern");
            object? valueCurrent = valuePatternObj == null ? null : ReadOrDefault<object?>(valuePatternObj, "Current", null);
            string valuePattern = valueCurrent == null ? string.Empty : ReadOrDefault(valueCurrent, "Value", string.Empty);
            bool valueIsReadOnly = valueCurrent != null && ReadOrDefault(valueCurrent, "IsReadOnly", false);

            object? scrollPatternObj = TryGetCurrentPatternObject(automationElement, "ScrollPattern");
            object? scrollCurrent = scrollPatternObj == null ? null : ReadOrDefault<object?>(scrollPatternObj, "Current", null);
            bool horizontallyScrollable = scrollCurrent != null && ReadOrDefault(scrollCurrent, "HorizontallyScrollable", false);
            bool verticallyScrollable = scrollCurrent != null && ReadOrDefault(scrollCurrent, "VerticallyScrollable", false);
            double scrollHorizontalViewSize = scrollCurrent == null ? 0.0 : ReadOrDefault(scrollCurrent, "HorizontalViewSize", 0.0);
            double scrollVerticalViewSize = scrollCurrent == null ? 0.0 : ReadOrDefault(scrollCurrent, "VerticalViewSize", 0.0);
            double scrollHorizontalPercent = scrollCurrent == null ? -1.0 : ReadOrDefault(scrollCurrent, "HorizontalScrollPercent", -1.0);
            double scrollVerticalPercent = scrollCurrent == null ? -1.0 : ReadOrDefault(scrollCurrent, "VerticalScrollPercent", -1.0);

            object? togglePatternObj = TryGetCurrentPatternObject(automationElement, "TogglePattern");
            object? toggleCurrent = togglePatternObj == null ? null : ReadOrDefault<object?>(togglePatternObj, "Current", null);
            long toggleState = toggleCurrent == null ? 0 : ReadOrDefault(toggleCurrent, "ToggleState", 0L);

            object? windowPatternObj = TryGetCurrentPatternObject(automationElement, "WindowPattern");
            object? windowCurrent = windowPatternObj == null ? null : ReadOrDefault<object?>(windowPatternObj, "Current", null);
            bool windowCanMaximize = windowCurrent == null || ReadOrDefault(windowCurrent, "CanMaximize", true);
            bool windowCanMinimize = windowCurrent == null || ReadOrDefault(windowCurrent, "CanMinimize", true);
            bool windowIsModal = windowCurrent != null && ReadOrDefault(windowCurrent, "IsModal", isModal);
            bool windowIsTopmost = windowCurrent != null && ReadOrDefault(windowCurrent, "IsTopmost", false);
            long windowInteraction = windowCurrent == null ? windowInteractionState : ReadOrDefault(windowCurrent, "WindowInteractionState", (long)windowInteractionState);
            long windowVisual = windowCurrent == null ? windowVisualState : ReadOrDefault(windowCurrent, "WindowVisualState", (long)windowVisualState);

            object? selectionPatternObj = TryGetCurrentPatternObject(automationElement, "SelectionPattern");
            object? selectionCurrent = selectionPatternObj == null ? null : ReadOrDefault<object?>(selectionPatternObj, "Current", null);
            bool selectionCanSelectMultiple = selectionCurrent != null && ReadOrDefault(selectionCurrent, "CanSelectMultiple", false);
            bool selectionIsSelectionRequired = selectionCurrent != null && ReadOrDefault(selectionCurrent, "IsSelectionRequired", false);
            object? selectionSelection = selectionPatternObj?.GetType().GetMethod("GetCurrentSelection", BindingFlags.Public | BindingFlags.Instance)?.Invoke(selectionPatternObj, null);

            object? selectionItemPatternObj = TryGetCurrentPatternObject(automationElement, "SelectionItemPattern");
            object? selectionItemCurrent = selectionItemPatternObj == null ? null : ReadOrDefault<object?>(selectionItemPatternObj, "Current", null);
            bool selectionItemIsSelected = selectionItemCurrent != null && ReadOrDefault(selectionItemCurrent, "IsSelected", false);
            object? selectionItemSelectionContainer = selectionItemCurrent == null ? null : ReadOrDefault<object?>(selectionItemCurrent, "SelectionContainer", null);

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
                Rect.from(x, y, Math.Max(1, width), Math.Max(1, height)),
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

        private static object? TryGetCurrentPatternObject(object automationElement, string patternTypeName)
        {
            Type? patternType = Type.GetType($"System.Windows.Automation.{patternTypeName}, UIAutomationClient");
            if (patternType == null)
            {
                return null;
            }

            object? patternIdentifier =
                patternType.GetProperty("Pattern", BindingFlags.Public | BindingFlags.Static)?.GetValue(null) ??
                patternType.GetField("Pattern", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
            if (patternIdentifier == null)
            {
                return null;
            }

            MethodInfo? tryGetPattern = automationElement.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(method =>
                {
                    if (!method.Name.Equals("TryGetCurrentPattern", StringComparison.Ordinal) || method.GetParameters().Length != 2)
                    {
                        return false;
                    }

                    ParameterInfo[] parameters = method.GetParameters();
                    return parameters[1].IsOut;
                });
            if (tryGetPattern == null)
            {
                return null;
            }

            object?[] args = { patternIdentifier, null };
            object? invokeResult = tryGetPattern.Invoke(automationElement, args);
            bool success = invokeResult is bool b && b;
            return success ? args[1] : null;
        }

        private static T ReadOrDefault<T>(object source, string propertyName, T defaultValue)
        {
            object? value = source.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)?.GetValue(source);
            if (value == null)
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
