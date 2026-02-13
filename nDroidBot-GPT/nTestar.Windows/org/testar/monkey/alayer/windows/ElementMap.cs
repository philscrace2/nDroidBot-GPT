using System.Collections.Concurrent;
using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.windows
{
    public sealed class ElementMap
    {
        private readonly ConcurrentDictionary<long, UIAElement> elementsByHandle = new();
        private readonly List<UIAElement> elementsByZIndex = new();

        public static Builder NewBuilder()
        {
            return new Builder();
        }

        public void Put(UIAElement element)
        {
            if (element.NativeWindowHandle != 0)
            {
                elementsByHandle[element.NativeWindowHandle] = element;
            }

            if (!elementsByZIndex.Contains(element))
            {
                elementsByZIndex.Add(element);
                SortByZIndexAndArea(elementsByZIndex);
            }
        }

        public bool TryGet(long hwnd, out UIAElement element)
        {
            return elementsByHandle.TryGetValue(hwnd, out element!);
        }

        public UIAElement? At(double x, double y)
        {
            foreach (UIAElement element in elementsByZIndex)
            {
                if (element.Contains(x, y))
                {
                    return element;
                }
            }

            return null;
        }

        public bool Obstructed(UIAElement element, double x, double y)
        {
            foreach (UIAElement obstacle in elementsByZIndex)
            {
                if (ReferenceEquals(obstacle, element) || obstacle.ZIndex <= element.ZIndex)
                {
                    continue;
                }

                if (obstacle.Contains(x, y))
                {
                    return true;
                }
            }

            return false;
        }

        public void Clear()
        {
            elementsByHandle.Clear();
            elementsByZIndex.Clear();
        }

        public sealed class Builder
        {
            private readonly List<UIAElement> elements = new();

            public Builder AddElement(UIAElement element)
            {
                if (Rect.area(element.Bounds) > 0.0)
                {
                    elements.Add(element);
                }

                return this;
            }

            public ElementMap Build()
            {
                SortByZIndexAndArea(elements);
                var map = new ElementMap();
                foreach (UIAElement element in elements)
                {
                    map.Put(element);
                }

                return map;
            }
        }

        private static void SortByZIndexAndArea(List<UIAElement> elements)
        {
            elements.Sort(static (left, right) =>
            {
                if (left.ZIndex < right.ZIndex)
                {
                    return 1;
                }

                if (left.ZIndex > right.ZIndex)
                {
                    return -1;
                }

                double leftArea = Rect.area(left.Bounds);
                double rightArea = Rect.area(right.Bounds);
                return leftArea < rightArea ? -1 : leftArea > rightArea ? 1 : 0;
            });
        }
    }
}
