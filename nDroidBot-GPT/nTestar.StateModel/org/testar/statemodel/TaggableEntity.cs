using System;
using System.Reflection;
using org.testar.monkey.alayer;

namespace org.testar.statemodel
{
    public abstract class TaggableEntity
    {
        private readonly TaggableBase _attributes;

        protected TaggableEntity()
        {
            _attributes = new TaggableBase();
        }

        public void AddAttribute(ITag attribute, object value)
        {
            try
            {
                SetTagValue(attribute, value);
            }
            catch (Exception)
            {
                Console.WriteLine($"Problem adding value for tag {attribute.name()} to abstract state");
            }
        }

        public TaggableBase GetAttributes()
        {
            return _attributes;
        }

        private void SetTagValue(ITag tag, object value)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            Type valueType = tag.type();
            MethodInfo? method = typeof(TaggableBase).GetMethod("set")?.MakeGenericMethod(valueType);
            if (method == null)
            {
                throw new InvalidOperationException("Unable to find tag set method.");
            }

            method.Invoke(_attributes, new[] { tag, value });
        }
    }
}
