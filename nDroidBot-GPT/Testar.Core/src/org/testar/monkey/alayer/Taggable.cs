namespace org.testar.monkey.alayer
{
    public interface Taggable
    {
        T get<T>(Tag<T> tag);
        T get<T>(Tag<T> tag, T defaultValue);
        void set<T>(Tag<T> tag, T value);
        void remove(ITag tag);
        IEnumerable<ITag> tags();
    }
}
