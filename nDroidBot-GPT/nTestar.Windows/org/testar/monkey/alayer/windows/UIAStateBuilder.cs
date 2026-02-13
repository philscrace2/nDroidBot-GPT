using org.testar.monkey.alayer;
using org.testar.serialisation;
using System.Threading;

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
                // UIA COM access is most reliable on an STA thread.
                if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
                {
                    return new StateFetcher(system).call();
                }

                UIAState? fetchedState = null;
                Exception? fetchException = null;
                var done = new ManualResetEventSlim(false);
                var thread = new Thread(() =>
                {
                    try
                    {
                        fetchedState = new StateFetcher(system).call();
                    }
                    catch (Exception ex)
                    {
                        fetchException = ex;
                    }
                    finally
                    {
                        done.Set();
                    }
                });

                thread.IsBackground = true;
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                done.Wait();

                if (fetchException != null)
                {
                    throw fetchException;
                }

                if (fetchedState != null)
                {
                    return fetchedState;
                }
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
