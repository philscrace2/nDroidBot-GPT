using org.testar.monkey;

namespace org.testar.monkey.alayer
{
    public interface Canvas
    {
        double width();
        double height();
        double x();
        double y();
        void begin();
        void end();
        void line(Pen pen, double x1, double y1, double x2, double y2);
        Pair<double, double>? textMetrics(Pen pen, string text);
        void text(Pen pen, double x, double y, double angle, string text);
        void clear(double x, double y, double width, double height);
        void triangle(Pen pen, double x1, double y1, double x2, double y2, double x3, double y3);
        void image(Pen pen, double x, double y, double width, double height, int[] image, int imageWidth, int imageHeight);
        void ellipse(Pen pen, double x, double y, double width, double height);
        void rect(Pen pen, double x, double y, double width, double height);
        Pen defaultPen();
        void release();
        void paintBatch();
    }
}
