namespace org.testar.monkey.alayer.devices
{
    public interface Mouse
    {
        void press(MouseButtons button);
        void release(MouseButtons button);
        void setCursor(double x, double y);
        Point cursor();
        void setCursorDisplayScale(double displayScale);
    }
}
