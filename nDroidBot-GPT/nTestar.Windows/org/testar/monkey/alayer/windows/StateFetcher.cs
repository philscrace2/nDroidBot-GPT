using System.Reflection;
using System.Runtime.InteropServices;
using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.windows
{
    public sealed class StateFetcher
    {
        private const int MaxNodes = 4000;

        // Java uses StateFetcher.call(); keep that shape for port parity.
        public UIAState call(SUT system)
        {
            var state = new UIAState();
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            string stateId = $"state-{timestamp}";
            long pid = system.get(Tags.PID, 0L);
            bool isRunning = system.get(Tags.IsRunning, true);
            bool isForeground = system.get(Tags.Foreground, true);
            bool hasMouse = system.get(Tags.HasStandardMouse, true);
            bool hasKeyboard = true;

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

            Rect rootBounds = ReadRootBounds(rootElement);
            var root = new UIARootElement(
                pid: pid,
                timeStamp: timestamp,
                bounds: rootBounds,
                isRunning: isRunning,
                isForeground: isForeground,
                hasStandardKeyboard: hasKeyboard,
                hasStandardMouse: hasMouse);
            state.SetRootElement(root);

            int visited = 0;
            int topIndex = 0;
            ElementMap.Builder elementMapBuilder = ElementMap.NewBuilder();
            foreach (object topLevel in EnumerateChildren(rootElement))
            {
                if (visited >= MaxNodes)
                {
                    break;
                }

                BuildTree(
                    topLevel,
                    state,
                    parentWidget: null,
                    parentElement: root,
                    path: $"/{topIndex}",
                    ref visited,
                    depth: 0,
                    elementMapBuilder);
                topIndex++;
            }

            if (topIndex == 0)
            {
                Console.WriteLine("StateFetcher: UIA root resolved but no child elements were returned.");
            }

            root.UpdateElementMap(elementMapBuilder.Build());
            state.set(Tags.MaxZIndex, Math.Max(0, topIndex - 1));
            return state;
        }

        // Backwards-compatible alias while callers are migrated.
        public UIAState Fetch(SUT system) => call(system);

        private static void BuildTree(
            object automationElement,
            UIAState state,
            UIAWidget? parentWidget,
            UIAElement? parentElement,
            string path,
            ref int visited,
            int depth,
            ElementMap.Builder elementMapBuilder)
        {
            if (visited >= MaxNodes)
            {
                return;
            }

            UIAElement? element = CreateElement(automationElement, depth);
            if (element == null)
            {
                return;
            }

            parentElement?.AddChild(element);
            if (depth == 0)
            {
                elementMapBuilder.AddElement(element);
            }

            var widget = new UIAWidget(element, $"w-{visited}");

            widget.set(Tags.Path, path);
            widget.set(Tags.HitTester, new UIAHitTester(widget));

            if (parentWidget == null)
            {
                state.addChild(widget);
            }
            else
            {
                parentWidget.addChild(widget);
            }

            visited++;
            int childIndex = 0;
            foreach (object child in EnumerateChildren(automationElement))
            {
                if (visited >= MaxNodes)
                {
                    break;
                }

                BuildTree(
                    child,
                    state,
                    widget,
                    element,
                    $"{path}/{childIndex}",
                    ref visited,
                    depth + 1,
                    elementMapBuilder);
                childIndex++;
            }
        }

        private static object? GetRootAutomationElement()
        {
            if (!OperatingSystem.IsWindows())
            {
                return null;
            }

            Type? automationElementType = ResolveUiaType("System.Windows.Automation.AutomationElement");
            if (automationElementType == null)
            {
                return null;
            }

            return automationElementType
                .GetProperty("RootElement", BindingFlags.Public | BindingFlags.Static)?
                .GetValue(null);
        }

        private static Rect ReadRootBounds(object rootAutomationElement)
        {
            UIAElement? parsedRoot = CreateElement(rootAutomationElement, zIndex: 0);
            if (parsedRoot != null)
            {
                return parsedRoot.Bounds;
            }

            return Rect.from(
                GetSystemMetrics(SM_XVIRTUALSCREEN),
                GetSystemMetrics(SM_YVIRTUALSCREEN),
                Math.Max(1, GetSystemMetrics(SM_CXVIRTUALSCREEN)),
                Math.Max(1, GetSystemMetrics(SM_CYVIRTUALSCREEN)));
        }

        private static UIAElement? CreateElement(object automationElement, int zIndex)
        {
            UIAElement? parsed = UIAElement.TryFromAutomationElement(automationElement);
            if (parsed == null)
            {
                return null;
            }

            parsed.SetZIndex(zIndex);
            return parsed;
        }

        private static IEnumerable<object> EnumerateChildren(object automationElement)
        {
            bool yielded = false;

            MethodInfo? findAll = automationElement.GetType().GetMethod("FindAll", BindingFlags.Public | BindingFlags.Instance);
            if (findAll != null)
            {
                object? trueCondition = findAll.GetParameters()[1].ParameterType
                    .GetProperty("TrueCondition", BindingFlags.Public | BindingFlags.Static)?
                    .GetValue(null);
                if (trueCondition != null)
                {
                    object treeScopeChildren = Enum.Parse(findAll.GetParameters()[0].ParameterType, "Children");
                    object? collection = findAll.Invoke(automationElement, new[] { treeScopeChildren, trueCondition });
                    if (collection != null)
                    {
                        Type collectionType = collection.GetType();
                        PropertyInfo? countProperty = collectionType.GetProperty("Count");
                        MethodInfo? getItem = collectionType.GetMethod("get_Item");
                        if (countProperty != null && getItem != null)
                        {
                            int count = (int)(countProperty.GetValue(collection) ?? 0);
                            for (int i = 0; i < count; i++)
                            {
                                object? child = getItem.Invoke(collection, new object[] { i });
                                if (child != null)
                                {
                                    yielded = true;
                                    yield return child;
                                }
                            }
                        }
                    }
                }
            }

            if (yielded)
            {
                yield break;
            }

            foreach (object child in EnumerateChildrenWithTreeWalker(automationElement))
            {
                yield return child;
            }
        }

        private static IEnumerable<object> EnumerateChildrenWithTreeWalker(object automationElement)
        {
            Type? treeWalkerType = ResolveUiaType("System.Windows.Automation.TreeWalker");
            if (treeWalkerType == null)
            {
                yield break;
            }

            object? controlViewWalker = treeWalkerType
                .GetProperty("ControlViewWalker", BindingFlags.Public | BindingFlags.Static)?
                .GetValue(null);
            if (controlViewWalker == null)
            {
                yield break;
            }

            MethodInfo? getFirstChild = treeWalkerType.GetMethod("GetFirstChild", BindingFlags.Public | BindingFlags.Instance);
            MethodInfo? getNextSibling = treeWalkerType.GetMethod("GetNextSibling", BindingFlags.Public | BindingFlags.Instance);
            if (getFirstChild == null || getNextSibling == null)
            {
                yield break;
            }

            object? child = getFirstChild.Invoke(controlViewWalker, new[] { automationElement });
            while (child != null)
            {
                yield return child;
                child = getNextSibling.Invoke(controlViewWalker, new[] { child });
            }
        }

        private const int SM_XVIRTUALSCREEN = 76;
        private const int SM_YVIRTUALSCREEN = 77;
        private const int SM_CXVIRTUALSCREEN = 78;
        private const int SM_CYVIRTUALSCREEN = 79;

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        private static Type? ResolveUiaType(string fullName)
        {
            Type? direct = Type.GetType($"{fullName}, UIAutomationClient");
            if (direct != null)
            {
                return direct;
            }

            Assembly? loaded = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => string.Equals(a.GetName().Name, "UIAutomationClient", StringComparison.OrdinalIgnoreCase));
            if (loaded != null)
            {
                return loaded.GetType(fullName);
            }

            try
            {
                Assembly assembly = Assembly.Load("UIAutomationClient");
                return assembly.GetType(fullName);
            }
            catch
            {
                return null;
            }
        }
    }
}
