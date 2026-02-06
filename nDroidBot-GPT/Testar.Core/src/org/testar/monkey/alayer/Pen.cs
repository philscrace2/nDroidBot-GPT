namespace org.testar.monkey.alayer
{
    [Serializable]
    public sealed class Pen
    {
        public static readonly Pen PEN_BLUE = new("Blue");
        public static readonly Pen PEN_RED = new("Red");
        public static readonly Pen PEN_BLACK = new("Black");

        public Pen(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public static Pen merge(Pen primary, Pen fallback)
        {
            return primary ?? fallback;
        }
    }
}
