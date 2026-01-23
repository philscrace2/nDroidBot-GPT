using System.Collections.Generic;
using System.Linq;
using org.testar.monkey;
using org.testar.monkey.alayer.exceptions;

namespace org.testar.monkey.alayer
{
    public sealed class StdAbstractor : Abstractor
    {
        private static readonly ITag[] tags = { Tags.Role, Tags.Title, Tags.ToolTipText, Tags.Enabled, Tags.Blocked };
        private static readonly double[] weights = { 2.0, 2.0, 2.0, 1.0, 1.0 };
        private static readonly double bestScore;

        static StdAbstractor()
        {
            bestScore = weights.Sum();
        }

        private sealed class StdFinder : Finder
        {
            private readonly Widget? cachedWidget;
            private readonly ITag[] tags;
            private readonly object?[] values;
            private readonly int[] indexPath;
            private readonly double[] weights;
            private readonly double minSimilarity;

            public StdFinder(ITag[] tags, object?[] values, double[] weights, int[] indexPath, double minSimilarity, Widget? cachedWidget)
            {
                this.tags = tags;
                this.values = values;
                this.weights = weights;
                this.indexPath = indexPath;
                this.minSimilarity = minSimilarity;
                this.cachedWidget = cachedWidget;
            }

            public Widget apply(Widget start)
            {
                Assert.notNull(start);
                if (cachedWidget != null && ReferenceEquals(cachedWidget.root(), start))
                {
                    return cachedWidget;
                }

                var candidates = new List<Widget>();
                double maxScore = 0.0;

                foreach (Widget widget in Util.makeIterable(start))
                {
                    double score = widgetSimilarity(widget);
                    if (score > maxScore)
                    {
                        candidates.Clear();
                        maxScore = score;
                        candidates.Add(widget);
                    }
                    else if (score == maxScore)
                    {
                        candidates.Add(widget);
                    }
                }

                if (maxScore / bestScore < minSimilarity)
                {
                    throw new WidgetNotFoundException();
                }

                if (candidates.Count == 1)
                {
                    return candidates[0];
                }

                Widget? bestCandidate = null;
                double bestPathScore = -1.0;
                foreach (Widget candidate in candidates)
                {
                    double score = indexPathSimilarity(indexPath, Util.indexPath(candidate));
                    if (score > bestPathScore)
                    {
                        bestCandidate = candidate;
                        bestPathScore = score;
                    }
                }

                return bestCandidate ?? candidates[0];
            }

            public Widget? getCachedWidget()
            {
                return cachedWidget;
            }

            private double widgetSimilarity(Widget other)
            {
                double score = 0.0;
                for (int i = 0; i < tags.Length; i++)
                {
                    object? value = GetTagValue(other, tags[i]);
                    score += (Util.equals(value, values[i]) ? 1.0 : 0.0) * weights[i];
                }

                return score;
            }

            private double indexPathSimilarity(int[] path1, int[] path2)
            {
                double score = 0.0;
                int length = Math.Min(path1.Length, path2.Length);
                for (int i = 0; i < length; i++)
                {
                    int a = path1[path1.Length - 1 - i];
                    int b = path2[path2.Length - 1 - i];
                    score += 1.0 / (Math.Abs(a - b) + 1.0);
                }

                return score;
            }
        }

        private readonly bool cache;
        private readonly double minSimilarity;

        public StdAbstractor() : this(true, 0.5)
        {
        }

        public StdAbstractor(bool cacheWidgetInFinder, double minSimilarity)
        {
            cache = cacheWidgetInFinder;
            this.minSimilarity = minSimilarity;
        }

        public Finder apply(Widget widget)
        {
            object?[] values = new object?[tags.Length];
            for (int i = 0; i < tags.Length; i++)
            {
                values[i] = GetTagValue(widget, tags[i]);
            }

            return new StdFinder(tags, values, weights, Util.indexPath(widget), minSimilarity, cache ? widget : null);
        }

        private static object? GetTagValue(Widget widget, ITag tag)
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
