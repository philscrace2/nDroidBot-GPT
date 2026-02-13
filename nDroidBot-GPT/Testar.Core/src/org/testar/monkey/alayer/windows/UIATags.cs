using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.windows
{
    public static class UIATags
    {
        private static readonly HashSet<ITag> uiaTags = new();
        private static readonly HashSet<ITag> patternAvailabilityTags = new();
        private static readonly Dictionary<ITag, HashSet<ITag>> controlPatternChildMapping = new();
        private static readonly Dictionary<ITag, bool> tagActiveMapping = new();

        public static readonly Tag<string> UIAName = from<string>("UIAName");
        public static readonly Tag<long> UIAControlType = from<long>("UIAControlType");
        public static readonly Tag<long> UIACulture = from<long>("UIACulture");
        public static readonly Tag<long> UIANativeWindowHandle = from<long>("UIANativeWindowHandle");
        public static readonly Tag<long> UIAOrientation = from<long>("UIAOrientation");
        public static readonly Tag<long> UIAProcessId = from<long>("UIAProcessId");
        public static readonly Tag<string> UIAFrameworkId = from<string>("UIAFrameworkId");
        public static readonly Tag<string> UIAAutomationId = from<string>("UIAAutomationId");
        public static readonly Tag<bool> UIAIsContentElement = from<bool>("UIAIsContentElement");
        public static readonly Tag<bool> UIAIsControlElement = from<bool>("UIAIsControlElement");
        public static readonly Tag<bool> UIAIsTopmostWindow = from<bool>("UIAIsTopmostWindow");
        public static readonly Tag<bool> UIAIsWindowModal = from<bool>("UIAIsWindowModal");
        public static readonly Tag<long> UIAWindowInteractionState = from<long>("UIAWindowInteractionState");
        public static readonly Tag<long> UIAWindowVisualState = from<long>("UIAWindowVisualState");
        public static readonly Tag<Rect> UIABoundingRectangle = from<Rect>("UIABoundingRectangle");
        public static readonly Tag<bool> UIAIsEnabled = from<bool>("UIAIsEnabled");
        public static readonly Tag<bool> UIAHasKeyboardFocus = from<bool>("UIAHasKeyboardFocus");
        public static readonly Tag<bool> UIAIsKeyboardFocusable = from<bool>("UIAIsKeyboardFocusable");
        public static readonly Tag<bool> UIAIsOffscreen = from<bool>("UIAIsOffscreen");
        public static readonly Tag<string> UIAHelpText = from<string>("UIAHelpText");
        public static readonly Tag<string> UIAClassName = from<string>("UIAClassName");
        public static readonly Tag<string> UIALocalizedControlType = from<string>("UIALocalizedControlType");
        public static readonly Tag<string> UIAItemType = from<string>("UIAItemType");
        public static readonly Tag<string> UIAItemStatus = from<string>("UIAItemStatus");
        public static readonly Tag<string> UIAProviderDescription = from<string>("UIAProviderDescription");
        public static readonly Tag<string> UIAAcceleratorKey = from<string>("UIAAcceleratorKey");
        public static readonly Tag<string> UIAAccessKey = from<string>("UIAAccessKey");

        public static readonly Tag<bool> UIAIsScrollPatternAvailable = registerPatternAvailability(
            from<bool>("UIAIsScrollPatternAvailable"));
        public static readonly Tag<bool> UIAIsTogglePatternAvailable = registerPatternAvailability(
            from<bool>("UIAIsTogglePatternAvailable"));
        public static readonly Tag<bool> UIAIsValuePatternAvailable = registerPatternAvailability(
            from<bool>("UIAIsValuePatternAvailable"));
        public static readonly Tag<bool> UIAIsWindowPatternAvailable = registerPatternAvailability(
            from<bool>("UIAIsWindowPatternAvailable"));
        public static readonly Tag<bool> UIAIsSelectionPatternAvailable = registerPatternAvailability(
            from<bool>("UIAIsSelectionPatternAvailable"));
        public static readonly Tag<bool> UIAIsSelectionItemPatternAvailable = registerPatternAvailability(
            from<bool>("UIAIsSelectionItemPatternAvailable"));

        public static readonly Tag<bool> UIAHorizontallyScrollable = from<bool>("UIAHorizontallyScrollable");
        public static readonly Tag<bool> UIAVerticallyScrollable = from<bool>("UIAVerticallyScrollable");
        public static readonly Tag<double> UIAScrollHorizontalViewSize = from<double>("UIAScrollHorizontalViewSize");
        public static readonly Tag<double> UIAScrollVerticalViewSize = from<double>("UIAScrollVerticalViewSize");
        public static readonly Tag<double> UIAScrollHorizontalPercent = from<double>("UIAScrollHorizontalPercent");
        public static readonly Tag<double> UIAScrollVerticalPercent = from<double>("UIAScrollVerticalPercent");

        public static readonly Tag<long> UIAToggleToggleState = from<long>("UIAToggleToggleState");
        public static readonly Tag<bool> UIAValueIsReadOnly = from<bool>("UIAValueIsReadOnly");
        public static readonly Tag<string> UIAValueValue = from<string>("UIAValueValue");

        public static readonly Tag<bool> UIAWindowCanMaximize = from<bool>("UIAWindowCanMaximize");
        public static readonly Tag<bool> UIAWindowCanMinimize = from<bool>("UIAWindowCanMinimize");
        public static readonly Tag<bool> UIAWindowIsModal = from<bool>("UIAWindowIsModal");
        public static readonly Tag<bool> UIAWindowIsTopmost = from<bool>("UIAWindowIsTopmost");
        public static readonly Tag<long> UIAWindowWindowInteractionState = from<long>("UIAWindowWindowInteractionState");
        public static readonly Tag<long> UIAWindowWindowVisualState = from<long>("UIAWindowWindowVisualState");

        public static readonly Tag<bool> UIASelectionCanSelectMultiple = from<bool>("UIASelectionCanSelectMultiple");
        public static readonly Tag<bool> UIASelectionIsSelectionRequired = from<bool>("UIASelectionIsSelectionRequired");
        public static readonly Tag<object> UIASelectionSelection = from<object>("UIASelectionSelection");
        public static readonly Tag<bool> UIASelectionItemIsSelected = from<bool>("UIASelectionItemIsSelected");
        public static readonly Tag<object> UIASelectionItemSelectionContainer = from<object>("UIASelectionItemSelectionContainer");

        static UIATags()
        {
            AddPatternChildren(
                UIAIsScrollPatternAvailable,
                UIAHorizontallyScrollable,
                UIAVerticallyScrollable,
                UIAScrollHorizontalViewSize,
                UIAScrollVerticalViewSize,
                UIAScrollHorizontalPercent,
                UIAScrollVerticalPercent);
            AddPatternChildren(UIAIsTogglePatternAvailable, UIAToggleToggleState);
            AddPatternChildren(UIAIsValuePatternAvailable, UIAValueIsReadOnly, UIAValueValue);
            AddPatternChildren(
                UIAIsWindowPatternAvailable,
                UIAWindowCanMaximize,
                UIAWindowCanMinimize,
                UIAWindowIsModal,
                UIAWindowIsTopmost,
                UIAWindowWindowInteractionState,
                UIAWindowWindowVisualState);
            AddPatternChildren(
                UIAIsSelectionPatternAvailable,
                UIASelectionCanSelectMultiple,
                UIASelectionIsSelectionRequired,
                UIASelectionSelection);
            AddPatternChildren(
                UIAIsSelectionItemPatternAvailable,
                UIASelectionItemIsSelected,
                UIASelectionItemSelectionContainer);
        }

        public static IReadOnlyCollection<ITag> getPatternAvailabilityTags()
        {
            return patternAvailabilityTags;
        }

        public static IReadOnlyCollection<ITag>? getChildTags(ITag patternAvailabilityTag)
        {
            return controlPatternChildMapping.TryGetValue(patternAvailabilityTag, out HashSet<ITag>? children)
                ? children
                : null;
        }

        public static bool tagIsActive(ITag tag)
        {
            return tagActiveMapping.TryGetValue(tag, out bool active) ? active : false;
        }

        public static void setTagActive(ITag tag, bool active)
        {
            tagActiveMapping[tag] = active;
        }

        public static IReadOnlyCollection<ITag> getAllActiveTags()
        {
            return tagActiveMapping.Where(pair => pair.Value).Select(pair => pair.Key).ToArray();
        }

        public static IReadOnlyCollection<ITag> getAllTags()
        {
            return uiaTags;
        }

        public static IReadOnlyCollection<ITag> getUIATags()
        {
            return uiaTags;
        }

        private static Tag<T> from<T>(string name)
        {
            Tag<T> tag = Tag<T>.from<T>(name, typeof(T));
            uiaTags.Add(tag);
            tagActiveMapping[tag] = true;
            return tag;
        }

        private static Tag<bool> registerPatternAvailability(Tag<bool> tag)
        {
            patternAvailabilityTags.Add(tag);
            return tag;
        }

        private static void AddPatternChildren(ITag patternAvailabilityTag, params ITag[] childTags)
        {
            var children = new HashSet<ITag>(childTags);
            controlPatternChildMapping[patternAvailabilityTag] = children;
        }
    }
}
