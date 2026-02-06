using System;
using System.Reflection;
using Action = org.testar.monkey.alayer.Action;
using org.testar.monkey.alayer;

namespace org.testar.statemodel
{
    public static class ConcreteActionFactory
    {
        public static ConcreteAction CreateConcreteAction(Action action, AbstractAction abstractAction)
        {
            ConcreteAction concreteAction = new ConcreteAction(action.get(Tags.ConcreteID), abstractAction);

            Widget originWidget = action.get(Tags.OriginWidget, null);
            if (originWidget != null)
            {
                SetAttributes(concreteAction, originWidget);
            }

            if (action.get(Tags.Desc, null) != null)
            {
                SetSpecificAttribute(concreteAction, Tags.Desc, action.get(Tags.Desc));
            }

            return concreteAction;
        }

        private static void SetAttributes(ModelWidget modelWidget, Widget testarWidget)
        {
            foreach (ITag tag in testarWidget.tags())
            {
                object value = GetTagValue(testarWidget, tag);
                modelWidget.AddAttribute(tag, value);
            }
        }

        private static void SetSpecificAttribute(ModelWidget modelWidget, ITag tagAttribute, object value)
        {
            modelWidget.AddAttribute(tagAttribute, value);
        }

        private static object GetTagValue(Taggable taggable, ITag tag)
        {
            Type valueType = tag.type();
            object defaultValue = valueType.IsValueType ? Activator.CreateInstance(valueType) : null;
            MethodInfo? method = taggable.GetType().GetMethod("get", new[] { tag.GetType(), valueType });
            if (method == null)
            {
                return null;
            }

            return method.Invoke(taggable, new[] { tag, defaultValue });
        }
    }
}
