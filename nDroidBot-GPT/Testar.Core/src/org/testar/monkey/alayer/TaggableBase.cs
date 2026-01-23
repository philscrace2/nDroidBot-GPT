using System.Collections.Generic;
using org.testar.monkey;
using org.testar.monkey.alayer.exceptions;

namespace org.testar.monkey.alayer
{
    [Serializable]
    public class TaggableBase : Taggable
    {
        private readonly Dictionary<ITag, object?> tagValues = Util.newHashMap<ITag, object?>();
        private bool allFetched;

        public T get<T>(Tag<T> tag)
        {
            T value = get(tag, default!);
            if (value == null)
            {
                throw new NoSuchTagException(tag);
            }

            return value;
        }

        public T get<T>(Tag<T> tag, T defaultValue)
        {
            Assert.notNull(tag);
            if (!tagValues.TryGetValue(tag, out var value) && !allFetched)
            {
                var fetched = fetch(tag);
                tagValues[tag] = fetched;
                value = fetched;
            }

            if (value == null)
            {
                return defaultValue;
            }

            return (T)value;
        }

        public IEnumerable<ITag> tags()
        {
            if (!allFetched)
            {
                foreach (var tag in tagDomain())
                {
                    if (!tagValues.ContainsKey(tag))
                    {
                        tagValues[tag] = null;
                    }
                }

                allFetched = true;
            }

            var ret = new List<ITag>();
            foreach (var kvp in tagValues)
            {
                if (kvp.Value != null)
                {
                    ret.Add(kvp.Key);
                }
            }

            return ret;
        }

        public void set<T>(Tag<T> tag, T value)
        {
            Assert.notNull(tag, value);
            if (!tag.type().IsInstanceOfType(value))
            {
                throw new ArgumentException("Value not of type required by this tag!");
            }

            tagValues[tag] = value;
        }

        public void remove(ITag tag)
        {
            tagValues[tag] = null;
        }

        protected virtual T? fetch<T>(Tag<T> tag)
        {
            return default;
        }

        protected virtual IEnumerable<ITag> tagDomain()
        {
            return new List<ITag>();
        }
    }
}
