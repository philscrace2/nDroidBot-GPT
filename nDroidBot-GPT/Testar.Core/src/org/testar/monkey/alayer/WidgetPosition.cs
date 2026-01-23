using org.testar.monkey;
using org.testar.monkey.alayer.exceptions;

namespace org.testar.monkey.alayer
{
    [Serializable]
    public sealed class WidgetPosition : AbstractPosition
    {
        private readonly Finder finder;
        private readonly double relX;
        private readonly double relY;
        private readonly ITag shapeTag;
        private readonly bool hitTest;
        private Point? cachedWidgetPoint;

        public static WidgetPosition fromFinder(Finder finder)
        {
            return fromFinder(finder, 0.5, 0.5);
        }

        public static WidgetPosition fromFinder(Finder finder, double relX, double relY)
        {
            return new WidgetPosition(finder, Tags.Shape, relX, relY, true);
        }

        public WidgetPosition(Finder finder, ITag shapeTag, double relX, double relY, bool hitTest)
        {
            Assert.notNull(finder, shapeTag);
            this.finder = finder;
            this.shapeTag = shapeTag;
            this.relX = relX;
            this.relY = relY;
            this.hitTest = hitTest;

            Widget? cachedWidget = finder.getCachedWidget();
            if (cachedWidget != null)
            {
                cachedWidgetPoint = Util.relToAbs((Shape)cachedWidget.get((Tag<Shape>)shapeTag), relX, relY);
            }
        }

        public override Point apply(State state)
        {
            try
            {
                Widget widget = finder.apply(state);
                if (hitTest && !Util.hitTest(widget, relX, relY, obscuredByChildEnabled))
                {
                    throw new PositionException("Widget found, but hittest failed!");
                }

                cachedWidgetPoint = Util.relToAbs((Shape)widget.get((Tag<Shape>)shapeTag), relX, relY);
                return cachedWidgetPoint;
            }
            catch (WidgetNotFoundException ex)
            {
                throw new PositionException(ex.Message);
            }
            catch (NoSuchTagException ex)
            {
                throw new PositionException(ex.Message);
            }
        }

        public override string ToString()
        {
            return cachedWidgetPoint == null ? $"({relX},{relY})" : cachedWidgetPoint.ToString();
        }
    }
}
