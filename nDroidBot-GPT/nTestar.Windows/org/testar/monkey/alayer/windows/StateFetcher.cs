using System.Reflection;
using System.Runtime.InteropServices;
using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.windows
{
    // Port shape aligned to Java: StateFetcher(system).call()
    public sealed class StateFetcher
    {
        private const int MaxNodes = 4000;
        private readonly SUT system;

        public StateFetcher(SUT system)
        {
            this.system = system;
        }

        public static UIARootElement buildRoot(SUT system)
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            bool isRunning = system.get(Tags.IsRunning, true);
            bool isForeground = system.get(Tags.Foreground, true);
            bool hasMouse = system.get(Tags.HasStandardMouse, system.get(Tags.StandardMouse, default(org.testar.monkey.alayer.devices.Mouse)) != null);
            bool hasKeyboard = system.get(Tags.StandardKeyboard, default(org.testar.monkey.alayer.devices.Keyboard)) != null;
            long pid = system.get(Tags.PID, 0L);

            Rect bounds = Rect.from(
                GetSystemMetrics(SM_XVIRTUALSCREEN),
                GetSystemMetrics(SM_YVIRTUALSCREEN),
                Math.Max(1, GetSystemMetrics(SM_CXVIRTUALSCREEN)),
                Math.Max(1, GetSystemMetrics(SM_CYVIRTUALSCREEN)));

            return new UIARootElement(
                pid: pid,
                timeStamp: timestamp,
                bounds: bounds,
                isRunning: isRunning,
                isForeground: isForeground,
                hasStandardKeyboard: hasKeyboard,
                hasStandardMouse: hasMouse);
        }

        public UIAState call()
        {
            UIARootElement uiaRoot = buildSkeleton(system);
            UIAState state = createWidgetTree(uiaRoot);
            state.set(Tags.Role, Roles.Process);
            state.set(Tags.NotResponding, false);
            return state;
        }

        // Compatibility shim while callers migrate.
        public UIAState call(SUT ignored) => call();

        private UIARootElement buildSkeleton(SUT sut)
        {
            UIARootElement uiaRoot = buildRoot(sut);
            if (!uiaRoot.IsRunning)
            {
                return uiaRoot;
            }

            object? rootAutomationElement = GetRootAutomationElement();
            if (rootAutomationElement == null)
            {
                return uiaRoot;
            }

            int visited = 0;
            int z = 0;
            ElementMap.Builder topLevelBuilder = ElementMap.NewBuilder();

            foreach (object topLevel in EnumerateChildren(rootAutomationElement))
            {
                if (visited >= MaxNodes)
                {
                    break;
                }

                UIAElement? topElement = BuildElementTree(topLevel, uiaRoot, depth: 0, ref visited);
                if (topElement == null)
                {
                    continue;
                }

                topElement.SetZIndex(z++);
                topLevelBuilder.AddElement(topElement);
            }

            uiaRoot.UpdateElementMap(topLevelBuilder.Build());
            return uiaRoot;
        }

        private static UIAElement? BuildElementTree(object automationElement, UIAElement parent, int depth, ref int visited)
        {
            if (visited >= MaxNodes)
            {
                return null;
            }

            UIAElement? element = UIAElement.TryFromAutomationElement(automationElement);
            if (element == null)
            {
                return null;
            }

            element.SetZIndex(depth);
            parent.AddChild(element);
            visited++;

            foreach (object child in EnumerateChildren(automationElement))
            {
                if (visited >= MaxNodes)
                {
                    break;
                }

                _ = BuildElementTree(child, element, depth + 1, ref visited);
            }

            return element;
        }

        private static UIAState createWidgetTree(UIARootElement root)
        {
            var state = new UIAState();
            state.SetRootElement(root);

            long timestamp = root.TimeStamp;
            string stateId = $"state-{timestamp}";
            state.set(Tags.ConcreteID, stateId);
            state.set(Tags.AbstractID, stateId);
            state.set(Tags.Abstract_R_ID, stateId);
            state.set(Tags.Abstract_R_T_ID, stateId);
            state.set(Tags.Abstract_R_T_P_ID, stateId);
            state.set(Tags.Title, "Desktop");
            state.set(Tags.OracleVerdict, Verdict.OK);
            state.set(Tags.TimeStamp, timestamp);
            state.set(Tags.MaxZIndex, 0.0);
            state.set(Tags.MinZIndex, 0.0);

            int index = 0;
            foreach (UIAElement childElement in root.Children)
            {
                createWidgetTree(state, childElement, $"/{index}");
                index++;
            }

            state.set(Tags.MaxZIndex, Math.Max(0, index - 1));
            return state;
        }

        private static void createWidgetTree(UIAState parent, UIAElement element, string path)
        {
            var widget = new UIAWidget(element, $"w-{path}");
            widget.set(Tags.Path, path);
            widget.set(Tags.HitTester, new UIAHitTester(widget));
            parent.addChild(widget);

            int childIndex = 0;
            foreach (UIAElement child in element.Children)
            {
                createWidgetTree(widget, child, $"{path}/{childIndex}");
                childIndex++;
            }
        }

        private static void createWidgetTree(UIAWidget parent, UIAElement element, string path)
        {
            var widget = new UIAWidget(element, $"w-{path}");
            widget.set(Tags.Path, path);
            widget.set(Tags.HitTester, new UIAHitTester(widget));
            parent.addChild(widget);

            int childIndex = 0;
            foreach (UIAElement child in element.Children)
            {
                createWidgetTree(widget, child, $"{path}/{childIndex}");
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

        private const int SM_XVIRTUALSCREEN = 76;
        private const int SM_YVIRTUALSCREEN = 77;
        private const int SM_CXVIRTUALSCREEN = 78;
        private const int SM_CYVIRTUALSCREEN = 79;

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);
    }
}
