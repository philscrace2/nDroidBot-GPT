namespace org.testar.statemodel.persistence.orientdb.util
{
    public static class Validation
    {
        public static bool HasValue(string? value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
    }
}
