using System;
using System.Collections.Generic;
using System.IO.Hashing;
using System.Text;
using org.testar.monkey.alayer;
using org.testar.statemodel.persistence.orientdb.entity;

namespace org.testar.statemodel.util
{
    public static class HydrationHelper
    {
        public static ITag? GetTag(TaggableBase tags, string tagName)
        {
            if (tags == null || string.IsNullOrEmpty(tagName))
            {
                return null;
            }

            foreach (ITag tag in tags.tags())
            {
                if (tag.name() == tagName)
                {
                    return tag;
                }
            }

            return null;
        }

        public static bool TypesMatch(Property property, Type type)
        {
            if (property == null || type == null)
            {
                return false;
            }

            return property.PropertyType == TypeConvertor.Instance.GetGraphType(type);
        }

        public static Property? GetProperty(ISet<Property> properties, string propertyName)
        {
            if (properties == null || string.IsNullOrEmpty(propertyName))
            {
                return null;
            }

            foreach (Property prop in properties)
            {
                if (prop.PropertyName == propertyName)
                {
                    return prop;
                }
            }

            return null;
        }

        public static string LowCollisionId(string text)
        {
            if (text == null)
            {
                text = string.Empty;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(text);
            uint crc = Crc32.HashToUInt32(bytes);
            string hash = JavaStringHashCode(text).ToString("x");
            string lengthHex = text.Length.ToString("x");
            return hash + lengthHex + crc.ToString();
        }

        public static string CreateGraphActionId(string sourceId, string targetId, string actionId, string modelIdentifier)
        {
            string id = sourceId + "-" + actionId + "-" + targetId + "-" + modelIdentifier;
            return LowCollisionId(id);
        }

        private static int JavaStringHashCode(string text)
        {
            unchecked
            {
                int hash = 0;
                foreach (char c in text)
                {
                    hash = 31 * hash + c;
                }
                return hash;
            }
        }
    }
}
