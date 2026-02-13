using org.testar.monkey;
using org.testar.monkey.alayer;
using org.testar.settings;
using Action = org.testar.monkey.alayer.Action;
using State = org.testar.monkey.alayer.State;

namespace org.testar
{
    public static class SutVisualization
    {
        public static void VisualizeSelectedAction(Settings settings, Canvas canvas, State state, Action action)
        {
            try
            {
                var visualizer = action.get(Tags.Visualizer, Util.NullVisualizer);
                double actionDuration = settings.get(ConfigTags.ActionDuration, 0.3);
                const int blinkCount = 3;
                double blinkDelay = actionDuration <= 0 ? 0.1 : actionDuration / blinkCount;

                for (int i = 0; i < blinkCount; i++)
                {
                    Util.pause(blinkDelay);
                    canvas.begin();
                    visualizer.run(state, canvas, Pen.PEN_BLACK);
                    canvas.end();

                    Util.pause(blinkDelay);
                    canvas.begin();
                    visualizer.run(state, canvas, Pen.PEN_RED);
                    canvas.end();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("visualizeSelectedAction : canvas visualization not available!");
                if (!string.IsNullOrWhiteSpace(ex.Message))
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static void visualizeActions(Canvas canvas, State state, System.Collections.Generic.ISet<Action> actions)
        {
            try
            {
                foreach (Action action in actions)
                {
                    var visualizer = action.get(Tags.Visualizer, Util.NullVisualizer);
                    visualizer.run(state, canvas, Pen.PEN_BLACK);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("visualizeActions : canvas visualization not available!");
                if (!string.IsNullOrWhiteSpace(ex.Message))
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static void visualizeFilteredActions(Canvas canvas, State state, System.Collections.Generic.ISet<Action> actions)
        {
            try
            {
                foreach (Action action in actions)
                {
                    var visualizer = action.get(Tags.Visualizer, Util.NullVisualizer);
                    visualizer.run(state, canvas, Pen.PEN_BLUE);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("visualizeFilteredActions : canvas visualization not available!");
                if (!string.IsNullOrWhiteSpace(ex.Message))
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
