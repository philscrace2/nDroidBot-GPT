using System;
using System.IO;
using System.Reflection;
using org.testar.monkey;
using org.testar.monkey.alayer;

namespace org.testar.statemodel
{
    public static class ConcreteStateFactory
    {
        public static ConcreteState CreateConcreteState(State newState, AbstractState abstractState, bool storeWidgets)
        {
            string concreteStateId = newState.get(Tags.ConcreteID);
            ConcreteState concreteState = new ConcreteState(concreteStateId, abstractState);

            SetAttributes(concreteState, newState);
            if (storeWidgets)
            {
                CopyWidgetTreeStructure(newState, concreteState, concreteState);
            }

            string srcPath = newState.get(Tags.ScreenshotPath, null);
            if (!string.IsNullOrWhiteSpace(srcPath))
            {
                string normalizePath = Path.GetFullPath(srcPath);

                for (int i = 0; i < 20 && !File.Exists(normalizePath); i++)
                {
                    Util.pause(0.1);
                }

                try
                {
                    if (File.Exists(normalizePath))
                    {
                        byte[] bytes = File.ReadAllBytes(normalizePath);
                        concreteState.SetScreenshot(bytes);
                    }
                    else
                    {
                        Console.WriteLine($"Screenshot file not found: {normalizePath}");
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex);
                }
            }

            return concreteState;
        }

        private static void SetAttributes(ModelWidget modelWidget, Widget testarWidget)
        {
            foreach (ITag tag in testarWidget.tags())
            {
                object value = GetTagValue(testarWidget, tag);
                modelWidget.AddAttribute(tag, value);
            }
        }

        private static void CopyWidgetTreeStructure(Widget testarWidget, ModelWidget modelWidget, ConcreteState rootWidget)
        {
            for (int i = 0; i < testarWidget.childCount(); i++)
            {
                Widget testarChildWidget = testarWidget.child(i);
                string widgetId = testarChildWidget.get(Tags.ConcreteID);
                ModelWidget newStateModelWidget = new ModelWidget(widgetId);
                newStateModelWidget.SetRootWidget(rootWidget);
                SetAttributes(newStateModelWidget, testarChildWidget);
                modelWidget.AddChild(newStateModelWidget);
                CopyWidgetTreeStructure(testarChildWidget, newStateModelWidget, rootWidget);
            }
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
