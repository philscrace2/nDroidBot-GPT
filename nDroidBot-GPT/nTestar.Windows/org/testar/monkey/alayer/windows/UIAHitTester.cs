using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.windows
{
    public class UIAHitTester : HitTester
    {
        public bool apply(double x, double y)
        {
            return true;
        }

        public bool apply(double x, double y, bool obscuredByChildFeature)
        {
            return true;
        }
    }
}
