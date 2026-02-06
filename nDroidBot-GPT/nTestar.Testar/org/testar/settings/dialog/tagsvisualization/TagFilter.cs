namespace org.testar.settings.dialog.tagsvisualization
{
    public abstract class TagFilter
    {
        private static TagFilter? instance;

        public static TagFilter getInstance()
        {
            return instance ?? throw new System.InvalidOperationException("TagFilter instance has not been initialized.");
        }

        public static void setInstance(TagFilter tagFilter)
        {
            instance = tagFilter;
        }

        public abstract bool visualizeTag(org.testar.monkey.alayer.ITag tag);
    }
}
