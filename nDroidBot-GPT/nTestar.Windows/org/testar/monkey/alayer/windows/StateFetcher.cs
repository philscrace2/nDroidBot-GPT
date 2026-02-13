using System.Reflection;
using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.windows
{
    public sealed class StateFetcher
    {
        public UIAState Fetch(SUT system)
        {
            var state = new UIAState();
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            string stateId = $"state-{timestamp}";

            state.set(Tags.ConcreteID, stateId);
            state.set(Tags.AbstractID, stateId);
            state.set(Tags.Abstract_R_ID, stateId);
            state.set(Tags.Abstract_R_T_ID, stateId);
            state.set(Tags.Abstract_R_T_P_ID, stateId);
            state.set(Tags.Title, "Desktop");
            state.set(Tags.Role, Roles.System);
            state.set(Tags.OracleVerdict, Verdict.OK);
            state.set(Tags.TimeStamp, timestamp);
            state.set(Tags.MaxZIndex, 0.0);
            state.set(Tags.MinZIndex, 0.0);

            int index = 0;
            foreach (UiElementInfo element in FetchTopLevelElements())
            {
                var widget = new UIAWidget(
                    concreteId: $"w-{index}",
                    title: string.IsNullOrWhiteSpace(element.Name) ? $"Widget {index}" : element.Name,
                    bounds: Rect.from(element.X, element.Y, Math.Max(1, element.Width), Math.Max(1, element.Height)),
                    role: MapRole(element.ControlType),
                    enabled: element.IsEnabled,
                    frameworkId: element.FrameworkId,
                    isModal: false,
                    hwnd: element.NativeWindowHandle);

                widget.set(Tags.Path, $"/{index}");
                state.addChild(widget);
                index++;
            }

            return state;
        }

        private static IEnumerable<UiElementInfo> FetchTopLevelElements()
        {
            if (!OperatingSystem.IsWindows())
            {
                yield break;
            }

            Type? automationElementType = Type.GetType("System.Windows.Automation.AutomationElement, UIAutomationClient");
            if (automationElementType == null)
            {
                yield break;
            }

            PropertyInfo? rootProperty = automationElementType.GetProperty("RootElement", BindingFlags.Public | BindingFlags.Static);
            object? rootElement = rootProperty?.GetValue(null);
            if (rootElement == null)
            {
                yield break;
            }

            MethodInfo? findAll = automationElementType.GetMethod("FindAll", BindingFlags.Public | BindingFlags.Instance);
            if (findAll == null)
            {
                yield break;
            }

            object? trueCondition = findAll.GetParameters()[1].ParameterType
                .GetProperty("TrueCondition", BindingFlags.Public | BindingFlags.Static)?
                .GetValue(null);
            if (trueCondition == null)
            {
                yield break;
            }

            object treeScopeChildren = Enum.Parse(findAll.GetParameters()[0].ParameterType, "Children");
            object? collection = findAll.Invoke(rootElement, new[] { treeScopeChildren, trueCondition });
            if (collection == null)
            {
                yield break;
            }

            Type collectionType = collection.GetType();
            PropertyInfo? countProperty = collectionType.GetProperty("Count");
            MethodInfo? getItem = collectionType.GetMethod("get_Item");
            if (countProperty == null || getItem == null)
            {
                yield break;
            }

            int count = (int)(countProperty.GetValue(collection) ?? 0);
            for (int i = 0; i < count; i++)
            {
                object? element = getItem.Invoke(collection, new object[] { i });
                if (element == null)
                {
                    continue;
                }

                UiElementInfo? info = ReadElementInfo(element);
                if (info != null && info.Width > 0 && info.Height > 0)
                {
                    yield return info;
                }
            }
        }

        private static UiElementInfo? ReadElementInfo(object automationElement)
        {
            PropertyInfo? currentProperty = automationElement.GetType().GetProperty("Current", BindingFlags.Public | BindingFlags.Instance);
            object? current = currentProperty?.GetValue(automationElement);
            if (current == null)
            {
                return null;
            }

            string name = ReadOrDefault(current, "Name", string.Empty);
            bool enabled = ReadOrDefault(current, "IsEnabled", true);
            string frameworkId = ReadOrDefault(current, "FrameworkId", string.Empty);
            int nativeWindowHandle = ReadOrDefault(current, "NativeWindowHandle", 0);

            object? controlType = ReadOrDefault<object?>(current, "ControlType", null);
            string controlTypeName = controlType?.GetType().GetProperty("ProgrammaticName", BindingFlags.Public | BindingFlags.Instance)?.GetValue(controlType)?.ToString()
                                     ?? "Control";

            object? rect = ReadOrDefault<object?>(current, "BoundingRectangle", null);
            if (rect == null)
            {
                return null;
            }

            double x = ReadOrDefault(rect, "X", 0.0);
            double y = ReadOrDefault(rect, "Y", 0.0);
            double width = ReadOrDefault(rect, "Width", 0.0);
            double height = ReadOrDefault(rect, "Height", 0.0);

            return new UiElementInfo(name, frameworkId, nativeWindowHandle, controlTypeName, enabled, x, y, width, height);
        }

        private static Role MapRole(string controlType)
        {
            if (controlType.Contains("Button", StringComparison.OrdinalIgnoreCase))
            {
                return Roles.Button;
            }

            if (controlType.Contains("Edit", StringComparison.OrdinalIgnoreCase) ||
                controlType.Contains("Document", StringComparison.OrdinalIgnoreCase))
            {
                return Roles.Text;
            }

            if (controlType.Contains("Window", StringComparison.OrdinalIgnoreCase))
            {
                return Roles.Dialog;
            }

            return Roles.Control;
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

        private sealed record UiElementInfo(
            string Name,
            string FrameworkId,
            long NativeWindowHandle,
            string ControlType,
            bool IsEnabled,
            double X,
            double Y,
            double Width,
            double Height);
    }
}
