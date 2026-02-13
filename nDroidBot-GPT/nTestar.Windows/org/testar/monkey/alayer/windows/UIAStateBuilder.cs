using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.windows
{
    public class UIAStateBuilder : StateBuilder
    {
        private readonly StateFetcher stateFetcher = new();
        private readonly TimeSpan timeout;

        public UIAStateBuilder()
            : this(TimeSpan.FromSeconds(10))
        {
        }

        public UIAStateBuilder(TimeSpan timeout)
        {
            this.timeout = timeout <= TimeSpan.Zero ? TimeSpan.FromSeconds(10) : timeout;
        }

        public State apply(SUT system)
        {
            try
            {
                Task<UIAState> fetchTask = Task.Run(() => stateFetcher.Fetch(system));
                if (fetchTask.Wait(timeout))
                {
                    return fetchTask.Result;
                }
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
