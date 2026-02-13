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
            double zIndex = 0.0)
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

            string valuePattern = string.Empty;

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
                windowInteractionState,
                windowVisualState,
                Rect.from(x, y, Math.Max(1, width), Math.Max(1, height)));
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
