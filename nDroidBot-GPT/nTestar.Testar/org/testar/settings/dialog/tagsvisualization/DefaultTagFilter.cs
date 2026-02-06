namespace org.testar.settings.dialog.tagsvisualization
{
    public class DefaultTagFilter : TagFilter
    {
        public override bool visualizeTag(org.testar.monkey.alayer.ITag tag)
        {
            return true;
        }
    }
}
