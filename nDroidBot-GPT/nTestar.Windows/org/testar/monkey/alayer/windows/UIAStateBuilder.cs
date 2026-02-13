using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.windows
{
    public class UIAStateBuilder : StateBuilder
    {
        private readonly StateFetcher stateFetcher = new();

        public UIAStateBuilder()
        {
        }

        public State apply(SUT system)
        {
            try
            {
                // Keep state fetching on the caller thread so SpyMode debugging is deterministic.
                return stateFetcher.call(system);
            }
            catch
            {
                // Keep TESTAR running if an individual state fetch fails.
            }

            var fallbackState = new UIAState();
            fallbackState.set(Tags.Role, Roles.Process);
            fallbackState.set(Tags.NotResponding, true);
            return fallbackState;
        }
    }
}
