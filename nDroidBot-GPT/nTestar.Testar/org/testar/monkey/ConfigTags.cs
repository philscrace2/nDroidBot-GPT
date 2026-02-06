namespace org.testar.monkey
{
    public static class ConfigTags
    {
        public static readonly org.testar.monkey.alayer.Tag<string> SUTConnectorValue =
            org.testar.monkey.alayer.Tag<string>.from<string>("SUTConnectorValue", typeof(string),
                "The connector value: executable path, windows title, process name");

        public static readonly org.testar.monkey.alayer.Tag<string> ApplicationName =
            org.testar.monkey.alayer.Tag<string>.from<string>("ApplicationName", typeof(string),
                "Name to identify the SUT.");

        public static readonly org.testar.monkey.alayer.Tag<string> ApplicationVersion =
            org.testar.monkey.alayer.Tag<string>.from<string>("ApplicationVersion", typeof(string),
                "Version to identify the SUT.");

        public static readonly org.testar.monkey.alayer.Tag<bool> JacocoCoverage =
            org.testar.monkey.alayer.Tag<bool>.from<bool>("JacocoCoverage", typeof(bool),
                "Enable JaCoCo coverage extractor.");

        public static readonly org.testar.monkey.alayer.Tag<string> JacocoCoverageIpAddress =
            org.testar.monkey.alayer.Tag<string>.from<string>("JacocoCoverageIpAddress", typeof(string),
                "JaCoCo agent host for coverage collection.");

        public static readonly org.testar.monkey.alayer.Tag<int> JacocoCoveragePort =
            org.testar.monkey.alayer.Tag<int>.from<int>("JacocoCoveragePort", typeof(int),
                "JaCoCo agent port for coverage collection.");

        public static readonly org.testar.monkey.alayer.Tag<string> JacocoCoverageClasses =
            org.testar.monkey.alayer.Tag<string>.from<string>("JacocoCoverageClasses", typeof(string),
                "Classpath filter for coverage report.");

        public static readonly org.testar.monkey.alayer.Tag<bool> JacocoCoverageAccumulate =
            org.testar.monkey.alayer.Tag<bool>.from<bool>("JacocoCoverageAccumulate", typeof(bool),
                "Accumulate coverage across sequences.");

        public static readonly org.testar.monkey.alayer.Tag<bool> ReportInHTML =
            org.testar.monkey.alayer.Tag<bool>.from<bool>("ReportInHTML", typeof(bool),
                "Enable HTML reporting.");

        public static readonly org.testar.monkey.alayer.Tag<bool> ReportInPlainText =
            org.testar.monkey.alayer.Tag<bool>.from<bool>("ReportInPlainText", typeof(bool),
                "Enable plain text reporting.");

        public static readonly org.testar.monkey.alayer.Tag<string> PathToReplaySequence =
            org.testar.monkey.alayer.Tag<string>.from<string>("PathToReplaySequence", typeof(string),
                "Path to the replay sequence file.");

        public static readonly org.testar.monkey.alayer.Tag<org.testar.monkey.Modes> Mode =
            org.testar.monkey.alayer.Tag<org.testar.monkey.Modes>.from<org.testar.monkey.Modes>(
                "Mode", typeof(org.testar.monkey.Modes), "Set the mode you want TESTAR to start in.");

        public static readonly org.testar.monkey.alayer.Tag<bool> CreateWidgetInfoJsonFile =
            org.testar.monkey.alayer.Tag<bool>.from<bool>("CreateWidgetInfoJsonFile", typeof(bool),
                "Enable widget info JSON export.");

        public static readonly org.testar.monkey.alayer.Tag<double> MaxReward =
            org.testar.monkey.alayer.Tag<double>.from<double>("MaxReward", typeof(double),
                "Max reward for Q-learning action selector.");

        public static readonly org.testar.monkey.alayer.Tag<double> Discount =
            org.testar.monkey.alayer.Tag<double>.from<double>("Discount", typeof(double),
                "Discount for Q-learning action selector.");

        public static readonly org.testar.monkey.alayer.Tag<bool> UseSystemActions =
            org.testar.monkey.alayer.Tag<bool>.from<bool>("UseSystemActions", typeof(bool),
                "Allow system actions.");

        public static readonly org.testar.monkey.alayer.Tag<System.Collections.Generic.List<string>> LlmTestGoals =
            org.testar.monkey.alayer.Tag<System.Collections.Generic.List<string>>.from<System.Collections.Generic.List<string>>(
                "LlmTestGoals", typeof(System.Collections.Generic.List<string>), "LLM test goals.");

        public static readonly org.testar.monkey.alayer.Tag<int> Sequences =
            org.testar.monkey.alayer.Tag<int>.from<int>("Sequences", typeof(int),
                "Number of sequences.");

        public static readonly org.testar.monkey.alayer.Tag<double> TimeToWaitAfterAction =
            org.testar.monkey.alayer.Tag<double>.from<double>("TimeToWaitAfterAction", typeof(double),
                "Wait time after action.");

        public static readonly org.testar.monkey.alayer.Tag<bool> FormFillingAction =
            org.testar.monkey.alayer.Tag<bool>.from<bool>("FormFillingAction", typeof(bool),
                "Enable form filling actions.");

        public static readonly org.testar.monkey.alayer.Tag<System.Collections.Generic.List<string>> ExtendedOracles =
            org.testar.monkey.alayer.Tag<System.Collections.Generic.List<string>>.from<System.Collections.Generic.List<string>>(
                "ExtendedOracles", typeof(System.Collections.Generic.List<string>), "Extended oracles list.");
    }
}
