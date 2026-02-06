using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using org.testar.monkey;
using org.testar.monkey.alayer;
using org.testar.monkey.alayer.visualizers;
using org.testar.stub;

namespace Testar.Core.Tests
{
    [TestFixture]
    public class TextVisualizerTests
    {
        private const string OriginalText = "original_text";
        private const string UpdatedText = "updated_text";

        [Test]
        public void CorrectOriginalTextVisualizer()
        {
            var visualizer = new TextVisualizer(new AbsolutePosition(1, 1), OriginalText, Pen.PEN_BLUE);
            Assert.That(visualizer.getText(), Is.EqualTo(OriginalText));
        }

        [Test]
        public void OriginalNullPositionThrows()
        {
            Assert.That(() => new TextVisualizer(null!, OriginalText, Pen.PEN_BLUE), Throws.ArgumentException);
        }

        [Test]
        public void OriginalNullTextThrows()
        {
            Assert.That(() => new TextVisualizer(new AbsolutePosition(1, 1), null!, Pen.PEN_BLUE), Throws.ArgumentException);
        }

        [Test]
        public void OriginalNullPenThrows()
        {
            Assert.That(() => new TextVisualizer(new AbsolutePosition(1, 1), OriginalText, null!), Throws.ArgumentException);
        }

        [Test]
        public void CorrectUpdatedTextVisualizer()
        {
            var visualizer = new TextVisualizer(new AbsolutePosition(1, 1), OriginalText, Pen.PEN_BLUE);
            visualizer = visualizer.withText(UpdatedText, Pen.PEN_RED);
            Assert.That(visualizer.getText(), Is.EqualTo(UpdatedText));
        }

        [Test]
        public void UpdatedNullTextThrows()
        {
            var visualizer = new TextVisualizer(new AbsolutePosition(1, 1), OriginalText, Pen.PEN_BLUE);
            Assert.That(() => visualizer.withText(null!, Pen.PEN_RED), Throws.ArgumentException);
        }

        [Test]
        public void UpdatedNullPenThrows()
        {
            var visualizer = new TextVisualizer(new AbsolutePosition(1, 1), OriginalText, Pen.PEN_BLUE);
            Assert.That(() => visualizer.withText(UpdatedText, null!), Throws.ArgumentException);
        }

        [Test]
        public void RunVisualizerDoesNotThrow()
        {
            var visualizer = new TextVisualizer(new AbsolutePosition(1, 1), OriginalText, Pen.PEN_BLUE);
            var canvas = new FakeCanvas(new Pair<double, double>(2.0, 2.0));
            visualizer.run(new StateStub(), canvas, Pen.PEN_RED);
        }

        [Test]
        public void RunVisualizerHandlesNullMetrics()
        {
            var visualizer = new TextVisualizer(new AbsolutePosition(1, 1), OriginalText, Pen.PEN_BLUE);
            var canvas = new FakeCanvas(null);
            visualizer.run(new StateStub(), canvas, Pen.PEN_RED);
        }

        [Test]
        public void RunNullStateThrows()
        {
            var visualizer = new TextVisualizer(new AbsolutePosition(1, 1), OriginalText, Pen.PEN_BLUE);
            Assert.That(() => visualizer.run(null!, new FakeCanvas(null), Pen.PEN_RED), Throws.ArgumentException);
        }

        [Test]
        public void RunNullCanvasThrows()
        {
            var visualizer = new TextVisualizer(new AbsolutePosition(1, 1), OriginalText, Pen.PEN_BLUE);
            Assert.That(() => visualizer.run(new StateStub(), null!, Pen.PEN_RED), Throws.ArgumentException);
        }

        [Test]
        public void RunNullPenThrows()
        {
            var visualizer = new TextVisualizer(new AbsolutePosition(1, 1), OriginalText, Pen.PEN_BLUE);
            Assert.That(() => visualizer.run(new StateStub(), new FakeCanvas(null), null!), Throws.ArgumentException);
        }

        private sealed class FakeCanvas : Canvas
        {
            private readonly Pair<double, double>? metrics;

            public FakeCanvas(Pair<double, double>? metrics)
            {
                this.metrics = metrics;
            }

            public double width()
            {
                return 0;
            }

            public double height()
            {
                return 0;
            }

            public double x()
            {
                return 0;
            }

            public double y()
            {
                return 0;
            }

            public void begin()
            {
            }

            public void end()
            {
            }

            public void line(Pen pen, double x1, double y1, double x2, double y2)
            {
            }

            public Pair<double, double>? textMetrics(Pen pen, string text)
            {
                return metrics;
            }

            public void text(Pen pen, double x, double y, double angle, string text)
            {
            }

            public void clear(double x, double y, double width, double height)
            {
            }

            public void triangle(Pen pen, double x1, double y1, double x2, double y2, double x3, double y3)
            {
            }

            public void image(Pen pen, double x, double y, double width, double height, int[] image, int imageWidth, int imageHeight)
            {
            }

            public void ellipse(Pen pen, double x, double y, double width, double height)
            {
            }

            public void rect(Pen pen, double x, double y, double width, double height)
            {
            }

            public Pen defaultPen()
            {
                return Pen.PEN_BLACK;
            }

            public void release()
            {
            }

            public void paintBatch()
            {
            }
        }
    }
}
