using System.Collections.Concurrent;
using org.testar.monkey;

namespace org.testar.monkey.alayer
{
    public interface ITag
    {
        string name();
        Type type();
        string getDescription();
    }

    [Serializable]
    public sealed class Tag<T> : ITag
    {
        private static readonly ConcurrentDictionary<TagKey, ITag> existingTags = new();

        private sealed record TagKey(string Name, Type Type);

        public static Tag<TValue> from<TValue>(string name, Type valueType)
        {
            Assert.notNull(name, valueType);
            var key = new TagKey(name, valueType);
            if (existingTags.TryGetValue(key, out var existing))
            {
                return (Tag<TValue>)existing;
            }

            var created = new Tag<TValue>(name, valueType);
            existingTags[key] = created;
            return created;
        }

        public static Tag<TValue> from<TValue>(string name, Type valueType, string description)
        {
            var tag = from<TValue>(name, valueType);
            tag.description = description;
            return tag;
        }

        private readonly Type clazz;
        private readonly string tagName;
        private int hashcode;
        private string description = string.Empty;

        private Tag(string name, Type clazz)
        {
            tagName = name;
            this.clazz = clazz;
        }

        public string name() => tagName;

        public Type type() => clazz;

        public string getDescription() => description;

        public override string ToString() => tagName;

        public override int GetHashCode()
        {
            if (hashcode == 0)
            {
                hashcode = tagName.GetHashCode() + 31 * Util.hashCode(clazz.FullName);
            }

            return hashcode;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is ITag other)
            {
                return tagName == other.name() && clazz == other.type();
            }

            return false;
        }

        public bool isOneOf(params ITag[] oneOf)
        {
            Assert.notNull(this, oneOf);
            foreach (var tag in oneOf)
            {
                if (Equals(tag))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
