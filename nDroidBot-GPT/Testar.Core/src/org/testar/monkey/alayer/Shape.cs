namespace org.testar.monkey.alayer
{
    public interface Shape
    {
        double x();
        double y();
        double width();
        double height();
        bool contains(double x, double y);
        void paint(Canvas canvas, Pen pen);
    }
}
