using System;
using System.Collections.Generic;
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
            uint crc = ComputeCrc32(bytes);
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

        private static uint ComputeCrc32(byte[] data)
        {
            uint crc = 0xFFFFFFFFu;
            foreach (byte b in data)
            {
                uint idx = (crc ^ b) & 0xFFu;
                crc = (crc >> 8) ^ Crc32Table[idx];
            }

            return crc ^ 0xFFFFFFFFu;
        }

        private static readonly uint[] Crc32Table = CreateCrc32Table();

        private static uint[] CreateCrc32Table()
        {
            const uint poly = 0xEDB88320u;
            var table = new uint[256];
            for (uint i = 0; i < table.Length; i++)
            {
                uint crc = i;
                for (int j = 0; j < 8; j++)
                {
                    crc = (crc & 1) != 0 ? (crc >> 1) ^ poly : crc >> 1;
                }

                table[i] = crc;
            }

            return table;
        }
    }
}
