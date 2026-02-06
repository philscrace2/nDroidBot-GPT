namespace org.testar
{
    public static class OutputStructure
    {
        public const string DATE_FORMAT = "yyyy-MM-dd_HH-mm-ss";

        public static string? startOuterLoopDateString;
        public static string? startInnerLoopDateString;

        public static string? executedSUTname;
        public static int sequenceInnerLoopCount;

        public static string? outerLoopOutputDir;
        public static string? screenshotsOutputDir;
        public static string? htmlOutputDir;
        public static string? logsOutputDir;
        public static string? debugLogsOutputDir;
        public static string? processListenerDir;

        public static void CalculateInnerLoopDateString()
        {
            calculateInnerLoopDateString();
        }

        public static void calculateOuterLoopDateString()
        {
            startOuterLoopDateString = string.Empty;
            string date = org.testar.monkey.Util.dateString(DATE_FORMAT);
            date = date + "s";
            date = date.Substring(0, 16) + "m" + date.Substring(17);
            date = date.Substring(0, 13) + "h" + date.Substring(14);
            string? hostname = System.Environment.GetEnvironmentVariable("HOSTNAME");
            if (!string.IsNullOrWhiteSpace(hostname))
            {
                startOuterLoopDateString = hostname + "_";
            }

            startOuterLoopDateString += date;
        }

        public static void calculateInnerLoopDateString()
        {
            string date = org.testar.monkey.Util.dateString(DATE_FORMAT);
            date = date + "s";
            date = date.Substring(0, 16) + "m" + date.Substring(17);
            date = date.Substring(0, 13) + "h" + date.Substring(14);
            startInnerLoopDateString = date;
        }

        public static void createOutputSUTname(settings.Settings settings)
        {
            executedSUTname = string.Empty;

            if (string.IsNullOrEmpty(settings.Get(org.testar.monkey.ConfigTags.ApplicationName, string.Empty)))
            {
                string sutConnectorValue = settings.Get(org.testar.monkey.ConfigTags.SUTConnectorValue, string.Empty);
                sutConnectorValue = sutConnectorValue.Replace("/", System.IO.Path.DirectorySeparatorChar.ToString());

                try
                {
                    if (sutConnectorValue.Contains("http") && sutConnectorValue.Contains("www."))
                    {
                        int indexWww = sutConnectorValue.IndexOf("www.", System.StringComparison.Ordinal) + 4;
                        int indexEnd = sutConnectorValue.IndexOf(".", indexWww, System.StringComparison.Ordinal);
                        string domain = sutConnectorValue.Substring(indexWww, indexEnd - indexWww);
                        executedSUTname = domain;
                    }
                    else if (sutConnectorValue.Contains(".exe", System.StringComparison.OrdinalIgnoreCase))
                    {
                        int startSut = sutConnectorValue.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1;
                        int endSut = sutConnectorValue.IndexOf(".exe", System.StringComparison.OrdinalIgnoreCase);
                        executedSUTname = sutConnectorValue.Substring(startSut, endSut - startSut);
                    }
                    else if (sutConnectorValue.Contains(".jar", System.StringComparison.OrdinalIgnoreCase))
                    {
                        int startSut = sutConnectorValue.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1;
                        int endSut = sutConnectorValue.IndexOf(".jar", System.StringComparison.OrdinalIgnoreCase);
                        executedSUTname = sutConnectorValue.Substring(startSut, endSut - startSut);
                    }
                }
                catch (System.Exception)
                {
                    System.Console.WriteLine("Warning: This run generation will be stored with empty name");
                }
            }
            else
            {
                executedSUTname = settings.Get(org.testar.monkey.ConfigTags.ApplicationName, string.Empty);
            }

            string version = settings.Get(org.testar.monkey.ConfigTags.ApplicationVersion, string.Empty);
            if (!string.IsNullOrEmpty(version))
            {
                executedSUTname += "_" + version;
            }
        }

        public static void createOutputFolders()
        {
            if (string.IsNullOrEmpty(startOuterLoopDateString) || string.IsNullOrEmpty(executedSUTname))
            {
                return;
            }

            outerLoopOutputDir = System.IO.Path.Combine(org.testar.monkey.Main.outputDir, $"{startOuterLoopDateString}_{executedSUTname}");
            System.IO.Directory.CreateDirectory(outerLoopOutputDir);

            if (!System.IO.Directory.Exists(outerLoopOutputDir))
            {
                outerLoopOutputDir = System.IO.Path.Combine(org.testar.monkey.Main.outputDir, $"{startOuterLoopDateString}_unknown");
                System.IO.Directory.CreateDirectory(outerLoopOutputDir);
            }

            screenshotsOutputDir = System.IO.Path.Combine(outerLoopOutputDir, "scrshots");
            System.IO.Directory.CreateDirectory(screenshotsOutputDir);

            htmlOutputDir = System.IO.Path.Combine(outerLoopOutputDir, "reports");
            System.IO.Directory.CreateDirectory(htmlOutputDir);

            logsOutputDir = System.IO.Path.Combine(outerLoopOutputDir, "logs");
            System.IO.Directory.CreateDirectory(logsOutputDir);

            debugLogsOutputDir = System.IO.Path.Combine(logsOutputDir, "debug");
            System.IO.Directory.CreateDirectory(debugLogsOutputDir);

            processListenerDir = System.IO.Path.Combine(logsOutputDir, "processListener");
            System.IO.Directory.CreateDirectory(processListenerDir);
        }
    }
}
