using System;

namespace Core.nTestar.Settings.Dialog.TagsVisualization
{
    public abstract class TagFilter
    {
        private static TagFilter? _instance;

        public static TagFilter Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException("TagFilter instance has not been initialized.");
                }

                return _instance;
            }
        }

        public static void SetInstance(TagFilter instance)
        {
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }
    }
}
