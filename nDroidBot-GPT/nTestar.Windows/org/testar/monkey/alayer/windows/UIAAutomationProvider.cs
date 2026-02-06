using org.testar.monkey.alayer;

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
            return new GDIScreenCanvas();
        }
    }
}
