using org.testar.monkey;

namespace org.testar.monkey.alayer
{
    [Serializable]
    public sealed class Verdict
    {
        public enum Severity
        {
            OK = 0,
            FAIL = 100
        }

        private readonly string infoValue;
        private readonly double severityValue;
        private readonly Visualizer visualizerValue;

        public static readonly Verdict OK = new Verdict(Severity.OK, "No problem detected.", Util.NullVisualizer);
        public static readonly Verdict FAIL = new Verdict(Severity.FAIL, "SUT failed.", Util.NullVisualizer);

        public Verdict(Severity severity, string info)
            : this(severity, info, Util.NullVisualizer)
        {
        }

        public Verdict(Severity severity, string info, Visualizer visualizer)
        {
            double value = ((int)severity) / 100.0;
            Assert.isTrue(value >= 0.0 && value <= 1.0);
            Assert.notNull(info, visualizer);
            severityValue = value;
            infoValue = info;
            visualizerValue = visualizer;
        }

        public double severity()
        {
            return severityValue;
        }

        public string info()
        {
            return infoValue;
        }

        public Visualizer visualizer()
        {
            return visualizerValue;
        }

        public string verdictSeverityTitle()
        {
            return severityValue >= 1.0 ? Severity.FAIL.ToString() : Severity.OK.ToString();
        }

        public override string ToString()
        {
            return $"severity: {severityValue} info: {infoValue}";
        }

        public Verdict join(Verdict verdict)
        {
            double maxSeverity = Math.Max(severityValue, verdict.severity());
            Severity joinedSeverity = maxSeverity >= 1.0 ? Severity.FAIL : Severity.OK;

            string joinedInfo = infoValue.Contains(verdict.info())
                ? infoValue
                : (severityValue == 0.0 ? string.Empty : infoValue + "\n") + verdict.info();

            Visualizer joinedVisualizer = severityValue >= verdict.severity() ? visualizerValue : verdict.visualizer();

            return new Verdict(joinedSeverity, joinedInfo, joinedVisualizer);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is Verdict other)
            {
                return severityValue.Equals(other.severityValue) &&
                       infoValue == other.infoValue &&
                       Equals(visualizerValue, other.visualizerValue);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(severityValue, infoValue, visualizerValue);
        }
    }
}
