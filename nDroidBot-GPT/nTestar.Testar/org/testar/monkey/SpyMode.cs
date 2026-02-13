using org.testar.monkey.alayer;
using org.testar.serialisation;
using State = org.testar.monkey.alayer.State;

namespace org.testar.monkey
{
    public class SpyMode
    {
        public void runSpyLoop(DefaultProtocol protocol)
        {
            SUT? system = null;
            Canvas? canvas = null;
            try
            {
                LogSerialiser.Log($"SpyMode: starting system{Environment.NewLine}", LogSerialiser.LogLevel.Info);
                system = protocol.StartSystemForLoop();
                LogSerialiser.Log($"SpyMode: system started{Environment.NewLine}", LogSerialiser.LogLevel.Info);
                canvas = protocol.EnsureCanvasForLoop();
                LogSerialiser.Log($"SpyMode: canvas {(canvas == null ? "null" : "ok")}{Environment.NewLine}", LogSerialiser.LogLevel.Info);

                while (protocol.mode() == Modes.Spy && protocol.IsSystemRunningForLoop(system))
                {
                    LogSerialiser.Log($"SpyMode: fetching state{Environment.NewLine}", LogSerialiser.LogLevel.Info);
                    State state = protocol.GetStateForLoop(system);
                    protocol.setStateForClickFilterLayerProtocol(state);
                    var actions = protocol.DeriveActionsForLoop(system, state);
                    LogSerialiser.Log($"SpyMode: state children={state.childCount()} actions={actions.Count}{Environment.NewLine}", LogSerialiser.LogLevel.Info);

                    if (canvas != null)
                    {
                        canvas.begin();
                        Util.clear(canvas);
                        protocol.VisualizeActionsForLoop(canvas, state, actions);
                        canvas.paintBatch();
                        canvas.end();
                    }

                    System.Threading.Thread.Sleep(protocol.SpyRefreshMs());
                }

                if (system != null && !protocol.IsSystemRunningForLoop(system))
                {
                    protocol.ExitSpyMode();
                }
            }
            catch (Exception ex)
            {
                LogSerialiser.Log($"SpyMode: loop failed: {ex}{Environment.NewLine}", LogSerialiser.LogLevel.Critical);
                throw;
            }
            finally
            {
                if (canvas != null)
                {
                    Util.clear(canvas);
                    canvas.end();
                }

                if (system != null)
                {
                    protocol.StopSystemForLoop(system);
                }
            }
        }
    }
}
