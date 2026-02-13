using System.Reflection;
using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.windows
{
    public sealed class StateFetcher
    {
        private const int MaxNodes = 4000;

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

            object? rootElement = GetRootAutomationElement();
            if (rootElement == null)
            {
                return state;
            }

            int visited = 0;
            int topIndex = 0;
            foreach (object topLevel in EnumerateChildren(rootElement))
            {
                if (visited >= MaxNodes)
                {
                    break;
                }

                BuildTree(topLevel, state, parent: null, path: $"/{topIndex}", ref visited, depth: 0);
                topIndex++;
            }

            state.set(Tags.MaxZIndex, Math.Max(0, topIndex - 1));
            return state;
        }

        private static void BuildTree(object automationElement, UIAState state, UIAWidget? parent, string path, ref int visited, int depth)
        {
            if (visited >= MaxNodes)
            {
                return;
            }

            UiElementInfo? info = ReadElementInfo(automationElement);
            if (info == null)
            {
                return;
            }

            var widget = new UIAWidget(
                concreteId: $"w-{visited}",
                title: string.IsNullOrWhiteSpace(info.Name) ? $"Widget {visited}" : info.Name,
                bounds: Rect.from(info.X, info.Y, Math.Max(1, info.Width), Math.Max(1, info.Height)),
                role: UIARoles.FromControlType(info.ControlType),
                enabled: info.IsEnabled,
                frameworkId: info.FrameworkId,
                isModal: info.IsModal,
                hwnd: info.NativeWindowHandle);

            widget.set(Tags.Path, path);
            widget.set(Tags.ZIndex, depth);
            widget.set(Tags.HitTester, new UIAHitTester(widget));

            if (parent == null)
            {
                state.addChild(widget);
            }
            else
            {
                parent.addChild(widget);
            }

            visited++;
            int childIndex = 0;
            foreach (object child in EnumerateChildren(automationElement))
            {
                if (visited >= MaxNodes)
                {
                    break;
                }

                BuildTree(child, state, widget, $"{path}/{childIndex}", ref visited, depth + 1);
                childIndex++;
            }
        }

        private static object? GetRootAutomationElement()
        {
            if (!OperatingSystem.IsWindows())
            {
                return null;
            }

            Type? automationElementType = Type.GetType("System.Windows.Automation.AutomationElement, UIAutomationClient");
            return automationElementType?.GetProperty("RootElement", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
        }

        private static IEnumerable<object> EnumerateChildren(object automationElement)
        {
            MethodInfo? findAll = automationElement.GetType().GetMethod("FindAll", BindingFlags.Public | BindingFlags.Instance);
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
            object? collection = findAll.Invoke(automationElement, new[] { treeScopeChildren, trueCondition });
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
                object? child = getItem.Invoke(collection, new object[] { i });
                if (child != null)
                {
                    yield return child;
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
            bool isModal = ReadOrDefault(current, "IsModal", false);

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

            return new UiElementInfo(name, frameworkId, nativeWindowHandle, controlTypeName, enabled, isModal, x, y, width, height);
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
            bool IsModal,
            double X,
            double Y,
            double Width,
            double Height);
    }
}
