using System.Runtime.InteropServices;

namespace org.testar.monkey.alayer.devices
{
    public class AWTMouse : Mouse
    {
        private Point cursorPoint = Point.from(0, 0);
        private double cursorDisplayScale = 1.0;

        public void press(MouseButtons button)
        {
            MouseEvent(button, up: false);
        }

        public void release(MouseButtons button)
        {
            MouseEvent(button, up: true);
        }

        public void setCursor(double x, double y)
        {
            cursorPoint = Point.from(x, y);
            if (OperatingSystem.IsWindows())
            {
                _ = SetCursorPos((int)Math.Round(x * cursorDisplayScale), (int)Math.Round(y * cursorDisplayScale));
            }
        }

        public Point cursor()
        {
            if (OperatingSystem.IsWindows() && GetCursorPos(out POINT p))
            {
                cursorPoint = Point.from(p.X / cursorDisplayScale, p.Y / cursorDisplayScale);
            }

            return cursorPoint;
        }

        public void setCursorDisplayScale(double displayScale)
        {
            cursorDisplayScale = displayScale > 0 ? displayScale : 1.0;
        }

        private static void MouseEvent(MouseButtons button, bool up)
        {
            if (!OperatingSystem.IsWindows())
            {
                return;
            }

            uint flag = button switch
            {
                MouseButtons.BUTTON1 => up ? MOUSEEVENTF_LEFTUP : MOUSEEVENTF_LEFTDOWN,
                MouseButtons.BUTTON2 => up ? MOUSEEVENTF_MIDDLEUP : MOUSEEVENTF_MIDDLEDOWN,
                MouseButtons.BUTTON3 => up ? MOUSEEVENTF_RIGHTUP : MOUSEEVENTF_RIGHTDOWN,
                _ => 0u
            };

            if (flag != 0)
            {
                mouse_event(flag, 0, 0, 0, UIntPtr.Zero);
            }
        }

        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        private const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const uint MOUSEEVENTF_MIDDLEUP = 0x0040;

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
    }
}
