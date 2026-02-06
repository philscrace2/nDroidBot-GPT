namespace org.testar.monkey.alayer.devices
{
    public class AWTMouse : Mouse
    {
        private Point cursorPoint = Point.from(0, 0);

        public void press(MouseButtons button)
        {
        }

        public void release(MouseButtons button)
        {
        }

        public void setCursor(double x, double y)
        {
            cursorPoint = Point.from(x, y);
        }

        public Point cursor()
        {
            return cursorPoint;
        }

        public void setCursorDisplayScale(double displayScale)
        {
        }
    }
}
