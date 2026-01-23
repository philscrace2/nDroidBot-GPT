using System.Collections.Generic;

namespace org.testar.monkey.alayer
{
    public abstract class TagsBase
    {
        protected static readonly HashSet<ITag> tagSet = new();

        protected static Tag<T> from<T>(string name, Type valueType)
        {
            var tag = Tag<T>.from<T>(name, valueType);
            tagSet.Add(tag);
            return tag;
        }

        public static IReadOnlyCollection<ITag> tagSetCollection()
        {
            return tagSet;
        }
    }
}
