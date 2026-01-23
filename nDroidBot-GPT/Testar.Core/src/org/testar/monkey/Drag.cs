namespace org.testar.monkey
{
    [Serializable]
    public sealed class Drag
    {
        public Drag(double fromX, double fromY, double toX, double toY)
        {
            FromX = fromX;
            FromY = fromY;
            ToX = toX;
            ToY = toY;
        }

        public double FromX { get; }
        public double FromY { get; }
        public double ToX { get; }
        public double ToY { get; }

        public double getFromX() => FromX;
        public double getFromY() => FromY;
        public double getToX() => ToX;
        public double getToY() => ToY;

        public override bool Equals(object? obj)
        {
            if (obj is not Drag other)
            {
                return false;
            }

            return FromX.Equals(other.FromX) &&
                   FromY.Equals(other.FromY) &&
                   ToX.Equals(other.ToX) &&
                   ToY.Equals(other.ToY);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FromX, FromY, ToX, ToY);
        }
    }
}
