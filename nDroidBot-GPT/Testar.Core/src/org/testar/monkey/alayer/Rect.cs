using org.testar.monkey;

namespace org.testar.monkey.alayer
{
    [Serializable]
    public sealed class Rect : Shape
    {
        private readonly double xValue;
        private readonly double yValue;
        private readonly double widthValue;
        private readonly double heightValue;

        public static bool intersect(Rect r1, Rect r2)
        {
            Assert.notNull(r1, r2);
            return !(r1.x() + r1.width() < r2.x() ||
                     r1.y() + r1.height() < r2.y() ||
                     r2.x() + r2.width() < r1.x() ||
                     r2.y() + r2.height() < r1.y());
        }

        public static bool contains(Rect r1, Rect r2)
        {
            Assert.notNull(r1, r2);
            return r2.x() >= r1.x() && r2.x() + r2.width() <= r1.x() + r1.width() &&
                   r2.y() >= r1.y() && r2.y() + r2.height() <= r1.y() + r1.height();
        }

        public static double area(Rect rect)
        {
            Assert.notNull(rect);
            return rect.width() * rect.height();
        }

        public static Rect? intersection(Rect? r1, Rect? r2)
        {
            if (r2 == null) return r1;
            if (r1 == null) return null;
            double x1 = Math.Max(r1.x(), r2.x());
            double x2 = Math.Min(r1.x() + r1.width(), r2.x() + r2.width());
            double y1 = Math.Max(r1.y(), r2.y());
            double y2 = Math.Min(r1.y() + r1.height(), r2.y() + r2.height());
            if (y2 < y1 || x2 < x1) return null;
            return fromCoordinates(x1, y1, x2, y2);
        }

        public static Rect from(double x, double y, double width, double height)
        {
            return new Rect(x, y, width, height);
        }

        public static Rect fromCoordinates(double x1, double y1, double x2, double y2)
        {
            return new Rect(x1, y1, x2 - x1, y2 - y1);
        }

        private Rect(double x, double y, double width, double height)
        {
            Assert.isTrue(width >= 0 && height >= 0,
                $"The width and height of a rectangle cannot be negative!: WIDTH={width} x HEIGHT={height}");
            xValue = x;
            yValue = y;
            widthValue = width;
            heightValue = height;
        }

        public double x() => xValue;
        public double y() => yValue;
        public double width() => widthValue;
        public double height() => heightValue;

        public bool contains(double x, double y)
        {
            if (x < xValue || y < yValue) return false;
            if (x > xValue + widthValue || y > yValue + heightValue) return false;
            return true;
        }

        public void paint(Canvas canvas, Pen pen)
        {
            canvas.rect(pen, xValue, yValue, widthValue, heightValue);
        }

        public override string ToString()
        {
            return $"Rect [x:{x()} y:{y()} w:{width()} h:{height()}]";
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is not Rect other) return false;
            return other.xValue == xValue && other.yValue == yValue &&
                   other.widthValue == widthValue && other.heightValue == heightValue;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(xValue, yValue, widthValue, heightValue);
        }
    }
}
