using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.windows
{
    public class UIAStateBuilder : StateBuilder
    {
        private readonly StateFetcher stateFetcher = new();

        public State apply(SUT system)
        {
            return stateFetcher.Fetch(system);
        }
    }
}
