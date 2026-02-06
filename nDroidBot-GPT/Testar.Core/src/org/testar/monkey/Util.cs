using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using org.testar.monkey.alayer;

namespace org.testar.monkey
{
    public static class Util
    {
        public static string lineSep => System.Environment.NewLine;

        public static readonly Visualizer NullVisualizer = new NullVisualizerImpl();

        public static string dateString(string format)
        {
            return DateTime.Now.ToString(format, CultureInfo.InvariantCulture);
        }

        public static bool equals(object? left, object? right)
        {
            return Equals(left, right);
        }

        public static int hashCode(object? value)
        {
            return value?.GetHashCode() ?? 0;
        }

        public static string toString(object? value)
        {
            return value?.ToString() ?? "null";
        }

        public static Dictionary<TKey, TValue> newHashMap<TKey, TValue>()
            where TKey : notnull
        {
            return new Dictionary<TKey, TValue>();
        }

        public static HashSet<T> newHashSet<T>()
        {
            return new HashSet<T>();
        }

        public static void clear(alayer.Canvas canvas)
        {
            canvas.clear(canvas.x(), canvas.y(), canvas.width(), canvas.height());
        }

        public static List<T> newArrayList<T>()
        {
            return new List<T>();
        }

        public static IEnumerable<org.testar.monkey.alayer.Widget> makeIterable(org.testar.monkey.alayer.Widget start)
        {
            var queue = new Queue<org.testar.monkey.alayer.Widget>();
            queue.Enqueue(start);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                yield return current;
                for (int i = 0; i < current.childCount(); i++)
                {
                    queue.Enqueue(current.child(i));
                }
            }
        }

        public static int[] indexPath(org.testar.monkey.alayer.Widget widget)
        {
            var path = new List<int>();
            var current = widget;
            while (current.parent() != null)
            {
                var parent = current.parent();
                if (parent == null)
                {
                    break;
                }

                int index = -1;
                for (int i = 0; i < parent.childCount(); i++)
                {
                    if (ReferenceEquals(parent.child(i), current))
                    {
                        index = i;
                        break;
                    }
                }

                if (index >= 0)
                {
                    path.Add(index);
                }

                current = parent;
            }

            path.Reverse();
            return path.ToArray();
        }

        public static bool isAncestorOf(org.testar.monkey.alayer.Widget ancestor, org.testar.monkey.alayer.Widget widget)
        {
            var current = widget.parent();
            while (current != null)
            {
                if (ReferenceEquals(current, ancestor))
                {
                    return true;
                }

                current = current.parent();
            }

            return false;
        }

        public static org.testar.monkey.alayer.Point relToAbs(org.testar.monkey.alayer.Shape shape, double relX, double relY)
        {
            return org.testar.monkey.alayer.Point.from(relToAbsX(shape, relX), relToAbsY(shape, relY));
        }

        public static double relToAbsX(org.testar.monkey.alayer.Shape shape, double relX)
        {
            return shape.x() + shape.width() * relX;
        }

        public static double relToAbsY(org.testar.monkey.alayer.Shape shape, double relY)
        {
            return shape.y() + shape.height() * relY;
        }

        public static bool hitTest(org.testar.monkey.alayer.Widget widget, double relX, double relY, bool obscuredByChildFeature)
        {
            var shape = widget.get(org.testar.monkey.alayer.Tags.Shape, default(org.testar.monkey.alayer.Shape));
            if (shape == null)
            {
                return false;
            }

            var abs = relToAbs(shape, relX, relY);
            var tester = widget.get(org.testar.monkey.alayer.Tags.HitTester, default(org.testar.monkey.alayer.HitTester));
            if (tester == null)
            {
                return false;
            }

            return tester.apply(abs.x(), abs.y(), obscuredByChildFeature);
        }

        public static bool hitTest(org.testar.monkey.alayer.Widget widget, double relX, double relY)
        {
            var shape = widget.get(org.testar.monkey.alayer.Tags.Shape, default(org.testar.monkey.alayer.Shape));
            if (shape == null)
            {
                return false;
            }

            var abs = relToAbs(shape, relX, relY);
            var tester = widget.get(org.testar.monkey.alayer.Tags.HitTester, default(org.testar.monkey.alayer.HitTester));
            if (tester == null)
            {
                return false;
            }

            return tester.apply(abs.x(), abs.y());
        }

        public static void pause(double seconds)
        {
            if (seconds <= 0)
            {
                return;
            }

            int ms = (int)(seconds * 1000.0);
            Thread.Sleep(ms);
        }

        public static string widgetDesc(org.testar.monkey.alayer.Widget widget, params org.testar.monkey.alayer.ITag[] tags)
        {
            if (tags == null || tags.Length == 0)
            {
                return widget.ToString() ?? string.Empty;
            }

            var parts = new List<string>();
            foreach (var tag in tags)
            {
                var value = GetTagValue(widget, tag);
                parts.Add($"{tag.name()}={value?.ToString() ?? string.Empty}");
            }

            return string.Join(" ", parts);
        }

        private sealed class NullVisualizerImpl : Visualizer
        {
            public void run(State state, Canvas canvas, Pen pen)
            {
            }
        }

        private static object? GetTagValue(org.testar.monkey.alayer.Widget widget, org.testar.monkey.alayer.ITag tag)
        {
            var method = widget.GetType().GetMethods()
                .FirstOrDefault(m => m.Name == "get" && m.IsGenericMethod && m.GetParameters().Length == 2);
            if (method == null)
            {
                return null;
            }

            var constructed = method.MakeGenericMethod(tag.type());
            return constructed.Invoke(widget, new object?[] { tag, null });
        }
    }
}
