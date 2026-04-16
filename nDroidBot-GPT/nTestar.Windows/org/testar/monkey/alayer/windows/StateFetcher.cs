using System.Collections.Concurrent;
using System.Windows.Automation;
using org.testar.monkey.alayer;
using org.testar.serialisation;

namespace org.testar.monkey.alayer.windows
{
    // Port shape aligned to Java: StateFetcher(system).call()
    public sealed class StateFetcher
    {
        private const int MaxNodes = 4000;
        private static readonly ConcurrentDictionary<long, AutomationProperty?> AutomationPropertyById = new();
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
            AutomationElement? rootAutomationElement = GetRootAutomationElement();

            Rect bounds = GetAutomationElementBounds(rootAutomationElement);

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
            LogSerialiser.Log(
                $"StateFetcher.call: top-level elements={uiaRoot.Children.Count} widgets={state.childCount()}{Environment.NewLine}",
                LogSerialiser.LogLevel.Info);
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

            AutomationElement? rootAutomationElement = GetRootAutomationElement();
            if (rootAutomationElement == null)
            {
                LogSerialiser.Log(
                    $"StateFetcher.buildSkeleton: UIAutomation root element not available.{Environment.NewLine}",
                    LogSerialiser.LogLevel.Critical);
                return uiaRoot;
            }

            int visited = 0;
            int z = 0;
            ElementMap.Builder topLevelBuilder = ElementMap.NewBuilder();
            long sutPid = sut.get(Tags.PID, 0L);

            foreach (AutomationElement topLevel in EnumerateTopLevelSutWindows(rootAutomationElement, sutPid, retryIfEmpty: true))
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
            LogSerialiser.Log(
                $"StateFetcher.buildSkeleton: collected top-level={uiaRoot.Children.Count}, visited={visited}{Environment.NewLine}",
                LogSerialiser.LogLevel.Info);
            return uiaRoot;
        }

        private static UIAElement? BuildElementTree(AutomationElement automationElement, UIAElement parent, int depth, ref int visited)
        {
            if (visited >= MaxNodes)
            {
                return null;
            }

            UIAElement? element;
            try
            {
                element = UIAElement.TryFromAutomationElement(automationElement);
            }
            catch
            {
                return null;
            }
            if (element == null)
            {
                return null;
            }

            try
            {
                PopulatePatternProperties(automationElement, element);
            }
            catch
            {
                // Keep tree build resilient: one bad pattern/property must not abort state capture.
            }
            element.SetZIndex(depth);
            parent.AddChild(element);
            visited++;

            foreach (AutomationElement child in EnumerateChildren(automationElement))
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

        private static AutomationElement? GetRootAutomationElement()
        {
            if (!OperatingSystem.IsWindows())
            {
                return null;
            }

            return AutomationElement.RootElement;
        }

        private static IEnumerable<AutomationElement> EnumerateChildren(AutomationElement automationElement)
        {
            bool yieldedAny = false;
            AutomationElementCollection? collection = null;
            try
            {
                collection = automationElement.FindAll(TreeScope.Children, Condition.TrueCondition);
            }
            catch
            {
                // Fall back to TreeWalker traversal below.
            }

            if (collection != null)
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    AutomationElement child = collection[i];
                    if (child != null)
                    {
                        yieldedAny = true;
                        yield return child;
                    }
                }
            }

            if (yieldedAny)
            {
                yield break;
            }

            foreach (AutomationElement child in EnumerateChildrenWithTreeWalker(automationElement))
            {
                yield return child;
            }
        }

        private static IEnumerable<AutomationElement> EnumerateChildrenWithTreeWalker(AutomationElement automationElement)
        {
            AutomationElement? child;
            try
            {
                child = TreeWalker.ControlViewWalker.GetFirstChild(automationElement);
            }
            catch
            {
                yield break;
            }
            while (child != null)
            {
                yield return child;
                try
                {
                    child = TreeWalker.ControlViewWalker.GetNextSibling(child);
                }
                catch
                {
                    yield break;
                }
            }
        }

        private static IEnumerable<AutomationElement> EnumerateTopLevelSutWindows(AutomationElement rootAutomationElement, long sutPid, bool retryIfEmpty)
        {
            const int maxAttempts = 6;
            int attempts = retryIfEmpty ? maxAttempts : 1;

            for (int attempt = 0; attempt < attempts; attempt++)
            {
                List<AutomationElement> filtered = EnumerateTopLevelWindows(rootAutomationElement, sutPid);
                if (filtered.Count > 0)
                {
                    return filtered;
                }

                if (attempt < attempts - 1)
                {
                    System.Threading.Thread.Sleep(150);
                }
            }

            return Array.Empty<AutomationElement>();
        }

        private static List<AutomationElement> EnumerateTopLevelWindows(AutomationElement rootAutomationElement, long sutPid)
        {
            List<AutomationElement> result = new();
            foreach (AutomationElement child in EnumerateChildren(rootAutomationElement))
            {
                if (MatchesSutProcess(child, sutPid))
                {
                    result.Add(child);
                }
            }

            return result;
        }

        private static bool MatchesSutProcess(AutomationElement element, long sutPid)
        {
            if (sutPid <= 0 || sutPid > int.MaxValue)
            {
                return true;
            }

            int targetPid = (int)sutPid;
            int currentPid;
            try
            {
                currentPid = element.Current.ProcessId;
            }
            catch
            {
                return false;
            }

            if (currentPid == targetPid)
            {
                return true;
            }

            // Some host windows do not expose the SUT PID on the top-level node.
            try
            {
                Condition descendantCondition = new PropertyCondition(AutomationElement.ProcessIdProperty, targetPid);
                return element.FindFirst(TreeScope.Descendants, descendantCondition) != null;
            }
            catch
            {
                return false;
            }
        }

        private static Rect GetAutomationElementBounds(AutomationElement? automationElement)
        {
            if (automationElement == null)
            {
                return Rect.from(0, 0, 1, 1);
            }

            try
            {
                System.Windows.Rect bounds = automationElement.Current.BoundingRectangle;
                if (!bounds.IsEmpty)
                {
                    return Rect.from(bounds.X, bounds.Y, Math.Max(1, bounds.Width), Math.Max(1, bounds.Height));
                }
            }
            catch
            {
                // Fall through to default bounds.
            }

            return Rect.from(0, 0, 1, 1);
        }

        private static void PopulatePatternProperties(AutomationElement automationElement, UIAElement uiaElement)
        {
            foreach (ITag patternTag in UIATags.getPatternAvailabilityTags())
            {
                long? patternAvailabilityId = UIAMapping.getPatternPropertyIdentifier(patternTag);
                if (patternAvailabilityId == null)
                {
                    continue;
                }

                object? availabilityValue = GetCurrentPropertyValue(automationElement, patternAvailabilityId.Value);
                setConvertedObjectValue(uiaElement, patternTag, availabilityValue);

                bool patternAvailable = availabilityValue is bool available && available;
                if (!patternAvailable)
                {
                    continue;
                }

                IReadOnlyCollection<ITag>? childTags = UIATags.getChildTags(patternTag);
                if (childTags == null)
                {
                    continue;
                }

                foreach (ITag childTag in childTags)
                {
                    long? childPropertyId = UIAMapping.getPatternPropertyIdentifier(childTag);
                    if (childPropertyId == null)
                    {
                        continue;
                    }

                    object? childValue = GetCurrentPropertyValue(automationElement, childPropertyId.Value);
                    setConvertedObjectValue(uiaElement, childTag, childValue);
                }
            }
        }

        private static object? GetCurrentPropertyValue(AutomationElement automationElement, long propertyId)
        {
            AutomationProperty? automationProperty = AutomationPropertyById.GetOrAdd(propertyId, id => LookupAutomationProperty((int)id));
            if (automationProperty == null)
            {
                return null;
            }

            object? value;
            try
            {
                value = automationElement.GetCurrentPropertyValue(automationProperty, false);
            }
            catch
            {
                return null;
            }

            return IsAutomationNotSupported(value) ? null : value;
        }

        private static AutomationProperty? LookupAutomationProperty(int propertyId)
        {
            try
            {
                return AutomationProperty.LookupById(propertyId);
            }
            catch
            {
                return null;
            }
        }

        private static bool IsAutomationNotSupported(object? value)
        {
            if (value == null)
            {
                return false;
            }

            return ReferenceEquals(AutomationElement.NotSupported, value);
        }

        private static void setConvertedObjectValue(UIAElement uiaElement, ITag tag, object? value)
        {
            if (value == null)
            {
                return;
            }

            if (tag.type() == typeof(bool))
            {
                if (TryConvertToBoolean(value, out bool boolValue))
                {
                    uiaElement.SetExtraTag(tag, boolValue);
                }

                return;
            }

            if (tag.type() == typeof(long))
            {
                if (TryConvertToLong(value, out long longValue))
                {
                    uiaElement.SetExtraTag(tag, longValue);
                }

                return;
            }

            if (tag.type() == typeof(double))
            {
                if (TryConvertToDouble(value, out double doubleValue))
                {
                    uiaElement.SetExtraTag(tag, doubleValue);
                }

                return;
            }

            if (tag.type() == typeof(string))
            {
                uiaElement.SetExtraTag(tag, value.ToString() ?? string.Empty);
                return;
            }

            // Keep complex/unknown UIA COM values as-is (Java keeps these as object).
            uiaElement.SetExtraTag(tag, value);
        }

        private static bool TryConvertToBoolean(object value, out bool converted)
        {
            if (value is bool b)
            {
                converted = b;
                return true;
            }

            try
            {
                converted = Convert.ToBoolean(value);
                return true;
            }
            catch
            {
                converted = false;
                return false;
            }
        }

        private static bool TryConvertToLong(object value, out long converted)
        {
            if (value is Enum enumValue)
            {
                converted = Convert.ToInt64(enumValue);
                return true;
            }

            if (value is long l)
            {
                converted = l;
                return true;
            }

            try
            {
                converted = Convert.ToInt64(value);
                return true;
            }
            catch
            {
                converted = 0L;
                return false;
            }
        }

        private static bool TryConvertToDouble(object value, out double converted)
        {
            if (value is double d)
            {
                converted = d;
                return true;
            }

            try
            {
                converted = Convert.ToDouble(value);
                return true;
            }
            catch
            {
                converted = 0.0;
                return false;
            }
        }
    }
}
