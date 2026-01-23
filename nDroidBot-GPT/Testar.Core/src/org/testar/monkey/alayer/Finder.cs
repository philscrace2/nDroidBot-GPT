namespace org.testar.monkey.alayer
{
    public interface Finder
    {
        Widget apply(Widget start);
        Widget? getCachedWidget();
    }
}
