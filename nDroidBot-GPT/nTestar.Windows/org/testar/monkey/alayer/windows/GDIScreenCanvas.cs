using org.testar.monkey.alayer;
using org.testar.monkey;

namespace org.testar.monkey.alayer.windows
{
    public class GDIScreenCanvas : Canvas
    {
        public double width() => 0;
        public double height() => 0;
        public double x() => 0;
        public double y() => 0;
        public void begin() { }
        public void end() { }
        public void line(Pen pen, double x1, double y1, double x2, double y2) { }
        public Pair<double, double>? textMetrics(Pen pen, string text) => Pair<double, double>.from(Math.Max(1, text?.Length ?? 0) * 7.0, 12.0);
        public void text(Pen pen, double x, double y, double angle, string text) { }
        public void clear(double x, double y, double width, double height) { }
        public void triangle(Pen pen, double x1, double y1, double x2, double y2, double x3, double y3) { }
        public void image(Pen pen, double x, double y, double width, double height, int[] image, int imageWidth, int imageHeight) { }
        public void ellipse(Pen pen, double x, double y, double width, double height) { }
        public void rect(Pen pen, double x, double y, double width, double height) { }
        public Pen defaultPen() => Pen.PEN_BLACK;
        public void release() { }
        public void paintBatch() { }
    }
}
