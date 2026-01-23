using System.Collections.Generic;
using System.Linq;
using org.testar.monkey.alayer;

namespace org.testar
{
    public static class StateManagementTags
    {
        public enum Group
        {
            General,
            ControlPattern,
            WebDriver
        }

        public static readonly Tag<string> WidgetControlType = Tag<string>.from<string>("Widget control type", typeof(string));

        private static readonly Dictionary<ITag, Group> TagGroups = new()
        {
            { WidgetControlType, Group.General }
        };

        public static bool isStateManagementTag(ITag tag)
        {
            return TagGroups.ContainsKey(tag);
        }

        public static Group getTagGroup(ITag tag)
        {
            return TagGroups.TryGetValue(tag, out var group) ? group : Group.General;
        }

        public static IEnumerable<ITag> getChildTags(ITag tag)
        {
            return Enumerable.Empty<ITag>();
        }

        public static HashSet<ITag> getAllTags()
        {
            return new HashSet<ITag>(TagGroups.Keys);
        }

        public static ITag? getTagFromSettingsString(string tagName)
        {
            return TagGroups.Keys.FirstOrDefault(tag => tag.name() == tagName);
        }
    }
}
