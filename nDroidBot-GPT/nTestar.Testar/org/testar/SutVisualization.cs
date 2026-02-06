using org.testar.monkey.alayer;
using State = org.testar.monkey.alayer.State;
using Action = org.testar.monkey.alayer.Action;
using org.testar.settings;

namespace org.testar
{
    public static class SutVisualization
    {
        public static void VisualizeSelectedAction(Settings settings, Canvas canvas, State state, Action action)
        {
            throw new System.NotImplementedException();
        }

        public static void visualizeActions(Canvas canvas, State state, System.Collections.Generic.ISet<Action> actions)
        {
        }

        public static void visualizeFilteredActions(Canvas canvas, State state, System.Collections.Generic.ISet<Action> actions)
        {
            // TODO: Implement visualization for filtered actions.
        }
    }
}
