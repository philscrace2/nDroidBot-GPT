using org.testar.monkey;
using org.testar.monkey.alayer;
using org.testar.monkey.alayer.exceptions;

namespace org.testar.monkey.alayer.visualizers
{
    public sealed class TextVisualizer : Visualizer
    {
        private readonly Position position;
        private readonly string text;
        private readonly Pen pen;

        public TextVisualizer(Position position, string text, Pen pen)
        {
            Assert.notNull(position, text, pen);
            this.position = position;
            this.text = text;
            this.pen = pen;
        }

        public string getText()
        {
            return text;
        }

        public TextVisualizer withText(string newText, Pen newPen)
        {
            Assert.notNull(newText, newPen);
            return new TextVisualizer(position, newText, newPen);
        }

        public void run(State state, Canvas canvas, Pen runPen)
        {
            Assert.notNull(state, canvas, runPen);
            Pen mergedPen = Pen.merge(runPen, pen);
            try
            {
                Point point = position.apply(state);
                Pair<double, double>? metrics = canvas.textMetrics(mergedPen, text);
                if (metrics == null)
                {
                    return;
                }

                canvas.text(mergedPen, point.x() - metrics.left() / 2, point.y() - metrics.right() / 2, 0, text);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
