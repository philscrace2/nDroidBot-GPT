namespace org.testar.statemodel.analysis.representation
{
    public class ActionViz
    {
        public ActionViz(string screenshotSource, string screenshotTarget, string actionDescription, int counterSource, int counterTarget, bool deterministic)
        {
            ScreenshotSource = screenshotSource;
            ScreenshotTarget = screenshotTarget;
            ActionDescription = actionDescription;
            CounterSource = counterSource;
            CounterTarget = counterTarget;
            Deterministic = deterministic;
        }

        public string ScreenshotSource { get; }

        public string ScreenshotTarget { get; }

        public string ActionDescription { get; }

        public int CounterSource { get; }

        public int CounterTarget { get; }

        public bool Deterministic { get; }
    }
}
