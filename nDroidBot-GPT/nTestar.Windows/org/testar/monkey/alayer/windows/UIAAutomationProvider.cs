using org.testar.monkey.alayer;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace org.testar.monkey.alayer.windows
{
    public class UIAAutomationProvider : IWindowsAutomationProvider
    {
        public StateBuilder CreateStateBuilder()
        {
            return new UIAStateBuilder();
        }

        public HitTester CreateHitTester()
        {
            return new UIAHitTester();
        }

        public Canvas CreateCanvas(Pen pen)
        {
            return new GDIScreenCanvas(pen);
        }

        public AWTCanvas CaptureScreenshot()
        {
            try
            {
                int x = GetSystemMetrics(SM_XVIRTUALSCREEN);
                int y = GetSystemMetrics(SM_YVIRTUALSCREEN);
                int width = Math.Max(1, GetSystemMetrics(SM_CXVIRTUALSCREEN));
                int height = Math.Max(1, GetSystemMetrics(SM_CYVIRTUALSCREEN));

                using var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(x, y, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
                }

                using var stream = new MemoryStream();
                bitmap.Save(stream, ImageFormat.Png);
                return new AWTCanvas(stream.ToArray());
            }
            catch
            {
                return new AWTCanvas();
            }
        }

        private const int SM_XVIRTUALSCREEN = 76;
        private const int SM_YVIRTUALSCREEN = 77;
        private const int SM_CXVIRTUALSCREEN = 78;
        private const int SM_CYVIRTUALSCREEN = 79;

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);
    }
}
