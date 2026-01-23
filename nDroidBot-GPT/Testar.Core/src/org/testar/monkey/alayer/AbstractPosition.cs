namespace org.testar.monkey.alayer
{
    [Serializable]
    public abstract class AbstractPosition : Position
    {
        protected bool obscuredByChildEnabled = true;

        public abstract Point apply(State state);

        public void obscuredByChildFeature(bool enable)
        {
            obscuredByChildEnabled = enable;
        }
    }
}
