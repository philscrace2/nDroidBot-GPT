using System.Collections.Concurrent;
using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.windows
{
    public static class UIAMapping
    {
        private static readonly ConcurrentDictionary<ITag, ITag> mappedStateTags = new();
        private static readonly ConcurrentDictionary<long, ITag> patternPropertyById = new();
        private static readonly ConcurrentDictionary<ITag, long> patternIdByProperty = new();

        static UIAMapping()
        {
            InitializePatternMappings();
        }

        private static void InitializePatternMappings()
        {
            RegisterPatternProperty(Windows.UIA_IsScrollPatternAvailablePropertyId, UIATags.UIAIsScrollPatternAvailable);
            RegisterPatternProperty(Windows.UIA_IsTogglePatternAvailablePropertyId, UIATags.UIAIsTogglePatternAvailable);
            RegisterPatternProperty(Windows.UIA_IsValuePatternAvailablePropertyId, UIATags.UIAIsValuePatternAvailable);
            RegisterPatternProperty(Windows.UIA_IsWindowPatternAvailablePropertyId, UIATags.UIAIsWindowPatternAvailable);
            RegisterPatternProperty(Windows.UIA_IsSelectionPatternAvailablePropertyId, UIATags.UIAIsSelectionPatternAvailable);
            RegisterPatternProperty(Windows.UIA_IsSelectionItemPatternAvailablePropertyId, UIATags.UIAIsSelectionItemPatternAvailable);
            RegisterPatternProperty(Windows.UIA_IsTablePatternAvailablePropertyId, UIATags.UIAIsTablePatternAvailable);
            RegisterPatternProperty(Windows.UIA_IsTableItemPatternAvailablePropertyId, UIATags.UIAIsTableItemPatternAvailable);

            RegisterPatternProperty(Windows.UIA_ScrollHorizontallyScrollablePropertyId, UIATags.UIAHorizontallyScrollable);
            RegisterPatternProperty(Windows.UIA_ScrollVerticallyScrollablePropertyId, UIATags.UIAVerticallyScrollable);
            RegisterPatternProperty(Windows.UIA_ScrollHorizontalViewSizePropertyId, UIATags.UIAScrollHorizontalViewSize);
            RegisterPatternProperty(Windows.UIA_ScrollVerticalViewSizePropertyId, UIATags.UIAScrollVerticalViewSize);
            RegisterPatternProperty(Windows.UIA_ScrollHorizontalScrollPercentPropertyId, UIATags.UIAScrollHorizontalPercent);
            RegisterPatternProperty(Windows.UIA_ScrollVerticalScrollPercentPropertyId, UIATags.UIAScrollVerticalPercent);

            RegisterPatternProperty(Windows.UIA_ToggleToggleStatePropertyId, UIATags.UIAToggleToggleState);
            RegisterPatternProperty(Windows.UIA_ValueIsReadOnlyPropertyId, UIATags.UIAValueIsReadOnly);
            RegisterPatternProperty(Windows.UIA_ValueValuePropertyId, UIATags.UIAValueValue);

            RegisterPatternProperty(Windows.UIA_WindowCanMaximizePropertyId, UIATags.UIAWindowCanMaximize);
            RegisterPatternProperty(Windows.UIA_WindowCanMinimizePropertyId, UIATags.UIAWindowCanMinimize);
            RegisterPatternProperty(Windows.UIA_WindowIsModalPropertyId, UIATags.UIAWindowIsModal);
            RegisterPatternProperty(Windows.UIA_WindowIsTopmostPropertyId, UIATags.UIAWindowIsTopmost);
            RegisterPatternProperty(Windows.UIA_WindowWindowInteractionStatePropertyId, UIATags.UIAWindowWindowInteractionState);
            RegisterPatternProperty(Windows.UIA_WindowWindowVisualStatePropertyId, UIATags.UIAWindowWindowVisualState);

            RegisterPatternProperty(Windows.UIA_SelectionCanSelectMultiplePropertyId, UIATags.UIASelectionCanSelectMultiple);
            RegisterPatternProperty(Windows.UIA_SelectionIsSelectionRequiredPropertyId, UIATags.UIASelectionIsSelectionRequired);
            RegisterPatternProperty(Windows.UIA_SelectionSelectionPropertyId, UIATags.UIASelectionSelection);
            RegisterPatternProperty(Windows.UIA_SelectionItemIsSelectedPropertyId, UIATags.UIASelectionItemIsSelected);
            RegisterPatternProperty(Windows.UIA_SelectionItemSelectionContainerPropertyId, UIATags.UIASelectionItemSelectionContainer);

            RegisterPatternProperty(Windows.UIA_TableColumnHeadersPropertyId, UIATags.UIATableColumnHeaders);
            RegisterPatternProperty(Windows.UIA_TableRowHeadersPropertyId, UIATags.UIATableRowHeaders);
            RegisterPatternProperty(Windows.UIA_TableRowOrColumnMajorPropertyId, UIATags.UIATableRowOrColumnMajor);
            RegisterPatternProperty(Windows.UIA_TableItemColumnHeaderItemsPropertyId, UIATags.UIATableItemColumnHeaderItems);
            RegisterPatternProperty(Windows.UIA_TableItemRowHeaderItemsPropertyId, UIATags.UIATableItemRowHeaderItems);
        }

        public static void RegisterMappedStateTag<T>(Tag<T> sourceTag, Tag<T> mappedTag)
        {
            mappedStateTags[sourceTag] = mappedTag;
        }

        public static Tag<T>? getMappedStateTag<T>(Tag<T> sourceTag)
        {
            if (mappedStateTags.TryGetValue(sourceTag, out ITag? mapped) && mapped is Tag<T> typedMapped)
            {
                return typedMapped;
            }

            return null;
        }

        public static void RegisterPatternProperty(long propertyId, ITag tag)
        {
            patternPropertyById[propertyId] = tag;
            patternIdByProperty[tag] = propertyId;
        }

        public static long? getPatternPropertyIdentifier(ITag patternPropertyTag)
        {
            return patternIdByProperty.TryGetValue(patternPropertyTag, out long id) ? id : null;
        }

        public static ITag? getPatternPropertyTag(long propertyId)
        {
            return patternPropertyById.TryGetValue(propertyId, out ITag? tag) ? tag : null;
        }

        public static void Clear()
        {
            mappedStateTags.Clear();
            patternPropertyById.Clear();
            patternIdByProperty.Clear();
            InitializePatternMappings();
        }
    }
}
