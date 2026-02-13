using org.testar.monkey.alayer;
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
                system = protocol.StartSystemForLoop();
                canvas = protocol.EnsureCanvasForLoop();

                while (protocol.mode() == Modes.Spy && protocol.IsSystemRunningForLoop(system))
                {
                    State state = protocol.GetStateForLoop(system);
                    protocol.setStateForClickFilterLayerProtocol(state);
                    var actions = protocol.DeriveActionsForLoop(system, state);

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
