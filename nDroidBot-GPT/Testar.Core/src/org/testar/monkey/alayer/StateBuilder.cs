namespace org.testar.monkey.alayer
{
    public interface StateBuilder
    {
        State apply(SUT system);
    }
}
