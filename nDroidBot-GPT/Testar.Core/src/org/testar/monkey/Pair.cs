namespace org.testar.monkey
{
    [Serializable]
    public sealed class Pair<L, R>
    {
        private readonly L leftValueField;
        private readonly R rightValueField;

        public static Pair<L, R> from(L left, R right)
        {
            return new Pair<L, R>(left, right);
        }

        public Pair(L left, R right)
        {
            leftValueField = left;
            rightValueField = right;
        }

        public L leftValue() => leftValueField;
        public R rightValue() => rightValueField;

        public L left() => leftValueField;
        public R right() => rightValueField;

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is Pair<L, R> other)
            {
                return Util.equals(leftValueField, other.left()) && Util.equals(rightValueField, other.right());
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Util.hashCode(leftValueField) + Util.hashCode(rightValueField);
        }

        public override string ToString()
        {
            return $"({Util.toString(leftValueField)}, {Util.toString(rightValueField)})";
        }
    }
}
