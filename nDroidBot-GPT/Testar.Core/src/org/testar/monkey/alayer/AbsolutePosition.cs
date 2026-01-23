using org.testar.monkey;

namespace org.testar.monkey.alayer
{
    [Serializable]
    public sealed class AbsolutePosition : AbstractPosition
    {
        private readonly Point point;

        public AbsolutePosition(double x, double y)
        {
            point = Point.from(x, y);
        }

        public AbsolutePosition(Point point)
        {
            Assert.notNull(point);
            this.point = point;
        }

        public override Point apply(State state)
        {
            return point;
        }

        public override string ToString()
        {
            return point.ToString();
        }
    }
}
