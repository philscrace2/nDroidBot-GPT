using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using org.testar.monkey.alayer;
using org.testar.monkey;
using DrawingBrush = System.Drawing.SolidBrush;
using DrawingColor = System.Drawing.Color;
using DrawingFont = System.Drawing.Font;
using DrawingPen = System.Drawing.Pen;

namespace org.testar.monkey.alayer.windows
{
    public class GDIScreenCanvas : Canvas
    {
        private readonly object sync = new();
        private readonly double xValue;
        private readonly double yValue;
        private readonly double widthValue;
        private readonly double heightValue;
        private readonly org.testar.monkey.alayer.Pen defaultPenValue;

        private Bitmap? frameBuffer;
        private Graphics? graphics;

        public GDIScreenCanvas()
            : this(org.testar.monkey.alayer.Pen.PEN_BLACK)
        {
        }

        public GDIScreenCanvas(org.testar.monkey.alayer.Pen defaultPen)
        {
            defaultPenValue = defaultPen ?? org.testar.monkey.alayer.Pen.PEN_BLACK;
            xValue = GetSystemMetrics(SM_XVIRTUALSCREEN);
            yValue = GetSystemMetrics(SM_YVIRTUALSCREEN);
            widthValue = Math.Max(1, GetSystemMetrics(SM_CXVIRTUALSCREEN));
            heightValue = Math.Max(1, GetSystemMetrics(SM_CYVIRTUALSCREEN));
        }

        public double width() => widthValue;
        public double height() => heightValue;
        public double x() => xValue;
        public double y() => yValue;

        public void begin()
        {
            lock (sync)
            {
                EnsureGraphics();
            }
        }

        public void end()
        {
            lock (sync)
            {
                graphics?.Flush();
            }
        }

        public void line(org.testar.monkey.alayer.Pen pen, double x1, double y1, double x2, double y2)
        {
            lock (sync)
            {
                EnsureGraphics();
                using var drawingPen = CreateDrawingPen(pen);
                graphics!.DrawLine(drawingPen, ToCanvasX(x1), ToCanvasY(y1), ToCanvasX(x2), ToCanvasY(y2));
            }
        }

        public Pair<double, double>? textMetrics(org.testar.monkey.alayer.Pen pen, string text)
        {
            lock (sync)
            {
                EnsureGraphics();
                using var font = CreateFont();
                SizeF size = graphics!.MeasureString(text ?? string.Empty, font);
                return Pair<double, double>.from(Math.Max(1.0, size.Width), Math.Max(1.0, size.Height));
            }
        }

        public void text(org.testar.monkey.alayer.Pen pen, double x, double y, double angle, string text)
        {
            lock (sync)
            {
                EnsureGraphics();
                using var brush = CreateDrawingBrush(pen);
                using var font = CreateFont();
                Graphics g = graphics!;

                float tx = ToCanvasX(x);
                float ty = ToCanvasY(y);
                GraphicsState state = g.Save();
                if (Math.Abs(angle) > double.Epsilon)
                {
                    g.TranslateTransform(tx, ty);
                    g.RotateTransform((float)angle);
                    g.DrawString(text ?? string.Empty, font, brush, 0, 0);
                }
                else
                {
                    g.DrawString(text ?? string.Empty, font, brush, tx, ty);
                }

                g.Restore(state);
            }
        }

        public void clear(double x, double y, double width, double height)
        {
            lock (sync)
            {
                EnsureGraphics();
                using var brush = new DrawingBrush(DrawingColor.Transparent);
                graphics!.CompositingMode = CompositingMode.SourceCopy;
                graphics.FillRectangle(brush, ToCanvasX(x), ToCanvasY(y), (float)Math.Max(0, width), (float)Math.Max(0, height));
                graphics.CompositingMode = CompositingMode.SourceOver;
            }
        }

        public void triangle(org.testar.monkey.alayer.Pen pen, double x1, double y1, double x2, double y2, double x3, double y3)
        {
            lock (sync)
            {
                EnsureGraphics();
                using var drawingPen = CreateDrawingPen(pen);
                graphics!.DrawPolygon(drawingPen, new[]
                {
                    new PointF(ToCanvasX(x1), ToCanvasY(y1)),
                    new PointF(ToCanvasX(x2), ToCanvasY(y2)),
                    new PointF(ToCanvasX(x3), ToCanvasY(y3))
                });
            }
        }

        public void image(org.testar.monkey.alayer.Pen pen, double x, double y, double width, double height, int[] image, int imageWidth, int imageHeight)
        {
            lock (sync)
            {
                EnsureGraphics();
                if (image == null || image.Length == 0 || imageWidth <= 0 || imageHeight <= 0)
                {
                    return;
                }

                using var source = new Bitmap(imageWidth, imageHeight, PixelFormat.Format32bppArgb);
                var bounds = new Rectangle(0, 0, imageWidth, imageHeight);
                BitmapData data = source.LockBits(bounds, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                try
                {
                    Marshal.Copy(image, 0, data.Scan0, Math.Min(image.Length, imageWidth * imageHeight));
                }
                finally
                {
                    source.UnlockBits(data);
                }

                graphics!.DrawImage(
                    source,
                    new RectangleF(ToCanvasX(x), ToCanvasY(y), (float)Math.Max(0, width), (float)Math.Max(0, height)));
            }
        }

        public void ellipse(org.testar.monkey.alayer.Pen pen, double x, double y, double width, double height)
        {
            lock (sync)
            {
                EnsureGraphics();
                using var drawingPen = CreateDrawingPen(pen);
                graphics!.DrawEllipse(drawingPen, ToCanvasX(x), ToCanvasY(y), (float)Math.Max(0, width), (float)Math.Max(0, height));
            }
        }

        public void rect(org.testar.monkey.alayer.Pen pen, double x, double y, double width, double height)
        {
            lock (sync)
            {
                EnsureGraphics();
                using var drawingPen = CreateDrawingPen(pen);
                graphics!.DrawRectangle(
                    drawingPen,
                    ToCanvasX(x),
                    ToCanvasY(y),
                    (float)Math.Max(0, width),
                    (float)Math.Max(0, height));
            }
        }

        public org.testar.monkey.alayer.Pen defaultPen() => defaultPenValue;

        public void release()
        {
            lock (sync)
            {
                graphics?.Dispose();
                frameBuffer?.Dispose();
                graphics = null;
                frameBuffer = null;
            }
        }

        public void paintBatch()
        {
            end();
        }

        private void EnsureGraphics()
        {
            if (graphics != null && frameBuffer != null)
            {
                return;
            }

            frameBuffer = new Bitmap((int)widthValue, (int)heightValue, PixelFormat.Format32bppArgb);
            graphics = Graphics.FromImage(frameBuffer);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            graphics.Clear(DrawingColor.Transparent);
        }

        private static DrawingPen CreateDrawingPen(org.testar.monkey.alayer.Pen? pen)
        {
            return new DrawingPen(ToDrawingColor(pen), 1.5f);
        }

        private static DrawingBrush CreateDrawingBrush(org.testar.monkey.alayer.Pen? pen)
        {
            return new DrawingBrush(ToDrawingColor(pen));
        }

        private static DrawingFont CreateFont()
        {
            return new DrawingFont("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Pixel);
        }

        private static DrawingColor ToDrawingColor(org.testar.monkey.alayer.Pen? pen)
        {
            string name = pen?.Name ?? string.Empty;
            if (name.Equals("Red", StringComparison.OrdinalIgnoreCase))
            {
                return DrawingColor.Red;
            }

            if (name.Equals("Blue", StringComparison.OrdinalIgnoreCase))
            {
                return DrawingColor.DeepSkyBlue;
            }

            if (name.Equals("Black", StringComparison.OrdinalIgnoreCase))
            {
                return DrawingColor.Black;
            }

            return DrawingColor.White;
        }

        private float ToCanvasX(double absoluteX) => (float)(absoluteX - xValue);
        private float ToCanvasY(double absoluteY) => (float)(absoluteY - yValue);

        private const int SM_XVIRTUALSCREEN = 76;
        private const int SM_YVIRTUALSCREEN = 77;
        private const int SM_CXVIRTUALSCREEN = 78;
        private const int SM_CYVIRTUALSCREEN = 79;

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);
    }
}
