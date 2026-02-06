using org.testar.monkey.alayer.devices;

namespace org.testar
{
    public interface IEventListener
    {
        void keyDown(KBKeys key);

        void keyUp(KBKeys key);

        void mouseDown(MouseButtons btn, double x, double y);

        void mouseUp(MouseButtons btn, double x, double y);

        void mouseMoved(double x, double y);
    }
}
