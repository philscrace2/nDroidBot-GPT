namespace org.testar.monkey.alayer
{
    public interface Visualizer
    {
        void run(State state, Canvas canvas, Pen pen);

        System.Collections.Generic.List<Shape> getShapes()
        {
            return new System.Collections.Generic.List<Shape> { Rect.from(0, 0, 0, 0) };
        }
    }
}
