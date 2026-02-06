using System.Collections.Generic;
using org.testar.monkey.alayer;

namespace org.testar.settings.dialog.tagsvisualization
{
    public class ConcreteTagFilter : TagFilter
    {
        public HashSet<ITag>? filter;

        public void setFilter(HashSet<ITag> newFilter)
        {
            filter = newFilter;
        }

        public override bool visualizeTag(ITag tag)
        {
            if (filter == null)
            {
                return false;
            }

            return filter.Contains(tag);
        }
    }
}
