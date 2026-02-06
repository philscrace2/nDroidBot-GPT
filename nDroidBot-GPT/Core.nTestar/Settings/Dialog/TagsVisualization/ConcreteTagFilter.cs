using System.Collections.Generic;
using org.testar.monkey.alayer;

namespace Core.nTestar.Settings.Dialog.TagsVisualization
{
    public sealed class ConcreteTagFilter : TagFilter
    {
        private HashSet<ITag>? filter;

        public void SetFilter(HashSet<ITag> newFilter)
        {
            filter = newFilter;
        }

        public override bool VisualizeTag(ITag tag)
        {
            if (filter == null)
            {
                return false;
            }

            return filter.Contains(tag);
        }
    }
}
