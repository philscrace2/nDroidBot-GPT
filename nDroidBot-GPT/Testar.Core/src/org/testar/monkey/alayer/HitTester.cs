namespace org.testar.monkey.alayer
{
    public interface HitTester
    {
        bool apply(double x, double y);
        bool apply(double x, double y, bool obscuredByChildFeature);
    }
}
