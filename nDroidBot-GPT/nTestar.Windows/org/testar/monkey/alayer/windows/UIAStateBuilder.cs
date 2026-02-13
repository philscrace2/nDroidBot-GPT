using org.testar.monkey.alayer;
using org.testar.serialisation;

namespace org.testar.monkey.alayer.windows
{
    public class UIAStateBuilder : StateBuilder
    {
        public UIAStateBuilder()
        {
        }

        public State apply(SUT system)
        {
            try
            {
                // Keep state fetching on the caller thread so SpyMode debugging is deterministic.
                return new StateFetcher(system).call();
            }
            catch (Exception ex)
            {
                LogSerialiser.Log(
                    $"UIAStateBuilder.apply: state fetch failed: {ex}{Environment.NewLine}",
                    LogSerialiser.LogLevel.Critical);
            }

            var fallbackState = new UIAState();
            fallbackState.set(Tags.Role, Roles.Process);
            fallbackState.set(Tags.NotResponding, true);
            return fallbackState;
        }
    }
}
