namespace org.testar.monkey.alayer
{
    public interface Position
    {
        Point apply(State state);
        void obscuredByChildFeature(bool enable);
    }
}
