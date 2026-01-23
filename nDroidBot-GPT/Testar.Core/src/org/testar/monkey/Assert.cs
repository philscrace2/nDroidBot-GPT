using System.Collections.Generic;

namespace org.testar.monkey
{
    public static class Assert
    {
        public static void isTrue(bool expression, string text)
        {
            if (!expression)
            {
                throw new ArgumentException(text);
            }
        }

        public static void isTrue(bool expression)
        {
            if (!expression)
            {
                throw new ArgumentException("You passed illegal parameters!");
            }
        }

        public static void isEquals(object expected, object actual)
        {
            if (!Equals(expected, actual))
            {
                throw new ArgumentException($"{expected} != {actual}");
            }
        }

        public static void isEquals(object expected, object actual, string text)
        {
            if (!Equals(expected, actual))
            {
                throw new ArgumentException(text);
            }
        }

        public static void notNull(object? obj, string text)
        {
            if (obj == null)
            {
                throw new ArgumentException(text);
            }
        }

        public static T notNull<T>(T? obj) where T : class
        {
            if (obj == null)
            {
                throw new ArgumentException("You passed null as a parameter!");
            }

            return obj;
        }

        public static void notNull(object? o1, object? o2)
        {
            if (o1 == null || o2 == null)
            {
                throw new ArgumentException("You passed null as a parameter!");
            }
        }

        public static void notNull(object? o1, object? o2, object? o3)
        {
            if (o1 == null || o2 == null || o3 == null)
            {
                throw new ArgumentException("You passed null as a parameter!");
            }
        }

        public static void hasText(string? text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("You passed a null or empty string!");
            }
        }

        public static void hasTextSetting(string? text, string settingName)
        {
            if (string.IsNullOrEmpty(text))
            {
                string message = "Non valid setting value!\n" +
                                 $"It seems that setting {settingName} as null or empty string!\n" +
                                 "Please provide a correct string value using TESTAR GUI or test.setting file";
                throw new ArgumentException(message);
            }
        }

        public static void collectionContains(ICollection<string> collection, string value)
        {
            if (!collection.Contains(value))
            {
                throw new ArgumentException($"Collection {collection} doesn't contain desired value {value}");
            }
        }

        public static void collectionNotContains(ICollection<string> collection, string value)
        {
            if (collection.Contains(value))
            {
                throw new ArgumentException($"Collection {collection} contains undesired value {value}");
            }
        }

        public static void collectionSize(ICollection<string> collection, int size)
        {
            if (collection.Count != size)
            {
                throw new ArgumentException($"Collection {collection} has undesired size {size}");
            }
        }
    }
}
