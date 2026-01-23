namespace org.testar.monkey.alayer
{
    [Serializable]
    public sealed class Point
    {
        private Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; }
        public double Y { get; }

        public static Point from(double x, double y)
        {
            return new Point(x, y);
        }

        public double x() => X;
        public double y() => Y;

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
