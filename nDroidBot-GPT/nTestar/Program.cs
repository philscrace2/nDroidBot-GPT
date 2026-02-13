// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Core.nTestar.Base;
using Core.nTestar.Settings.Dialog.TagsVisualization;
using Core.nTestar.Startup;
using org.testar;
using org.testar.environment;
using org.testar.monkey.alayer;
using org.testar.monkey.alayer.windows;
using TestarEnvironment = org.testar.environment.Environment;


public class MainClass
{
    public const string TESTAR_VERSION = "2.6.20 (14-Aug-2024)";
    public const string SETTINGS_FILE = "test.testarsettings";
    public const string SUT_SETTINGS_EXT = ".sse";
    public static string SSE_ACTIVATED = null;

    public static string testarDir = (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".")
        + Path.DirectorySeparatorChar;
    public static string settingsDir = testarDir + "settings" + Path.DirectorySeparatorChar;
    public static string outputDir = testarDir + "output" + Path.DirectorySeparatorChar;
    public static string tempDir = outputDir + "temp" + Path.DirectorySeparatorChar;
    private static SseManager? sseManager;

    public static string[] GetSSE()
    {
        sseManager ??= new SseManager(settingsDir, SETTINGS_FILE, SUT_SETTINGS_EXT);
        return sseManager.GetSseFiles();
    }

    public static string GetTestSettingsFile()
    {
        if (string.IsNullOrWhiteSpace(SSE_ACTIVATED))
        {
            throw new InvalidOperationException("SSE has not been activated.");
        }

        return Path.Combine(settingsDir, SSE_ACTIVATED, SETTINGS_FILE);
    }

    public static void Main(string[] args)
    {
        //IsValidEnvironment();
        //VerifyTestarInitialDirectory();
        InitTagVisualization();
        InitTestarSSE(args);

        string testSettingsFileName = GetTestSettingsFile();
        Console.WriteLine($"TESTAR version is <{TESTAR_VERSION}>");
        Console.WriteLine($"Test settings is <{testSettingsFileName}>");

        Settings settings = Settings.LoadSettings(args, testSettingsFileName);

        if (!bool.TryParse(settings.Get("ShowVisualSettingsDialogOnStartup", "false"), out bool showDialog) || !showDialog)
        {
            SetTestarDirectory(settings);
            InitCodingManager(settings);
            InitOperatingSystem();
            StartTestar(settings);
        }
        else
        {
            while (StartTestarDialog(settings, testSettingsFileName))
            {
                testSettingsFileName = GetTestSettingsFile();
                settings = Settings.LoadSettings(args, testSettingsFileName);
                SetTestarDirectory(settings);
                InitCodingManager(settings);
                InitOperatingSystem();
                StartTestar(settings);
            }
        }

        StopTestar();
    }

    private static void VerifyTestarInitialDirectory()
    {
        if (!File.Exists(Path.Combine(testarDir, "testar.bat")))
        {
            Console.WriteLine("WARNING: testar.bat not found in the current directory.");
            System.Environment.Exit(-1);
        }
    }

    private static void SetTestarDirectory(Settings settings)
    {
        outputDir = settings.Get("OutputDir", outputDir);
        tempDir = settings.Get("TempDir", tempDir);
    }

    private static void InitTestarSSE(string[] args)
    {
        // Keep startup locale aligned with Java TESTAR so settings parsing remains predictable.
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

        sseManager = new SseManager(settingsDir, SETTINGS_FILE, SUT_SETTINGS_EXT);

        // Allow users to use command line to choose a protocol by modifying/creating the .sse file.
        foreach (string arg in args)
        {
            if (arg.Contains("sse=", StringComparison.Ordinal))
            {
                try
                {
                    sseManager.ProtocolFromCmd(arg);
                }
                catch (IOException)
                {
                    Console.WriteLine("Error trying to modify sse from command line");
                }
            }
        }

        string[] files = GetSSE();

        // If there is more than one .sse file, delete them all.
        if (files.Length > 1)
        {
            Console.WriteLine("Too many *.sse files - exactly one expected!");
            foreach (string file in files)
            {
                string filePath = Path.Combine(settingsDir, file);
                bool deleted = false;
                try
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        deleted = true;
                    }
                }
                catch (IOException)
                {
                    deleted = false;
                }

                Console.WriteLine($"Delete file <{file}> = {deleted}");
            }

            files = Array.Empty<string>();
        }

        // If the protocol of selected .sse file does not exist, delete it.
        if (files.Length == 1 && !ExistsSSE(ExtractSSEName(files[0])))
        {
            Console.WriteLine("Protocol of indicated .sse file does not exist");
            string filePath = Path.Combine(settingsDir, files[0]);
            bool deleted = false;
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    deleted = true;
                }
            }
            catch (IOException)
            {
                deleted = false;
            }

            Console.WriteLine($"Delete file <{files[0]}> = {deleted}");
            files = Array.Empty<string>();
        }

        // If there is no .sse file, show selector and create one.
        if (files.Length == 0)
        {
            SettingsSelection();
        }
        else
        {
            SSE_ACTIVATED = ExtractSSEName(files[0]);
        }

        if (string.IsNullOrWhiteSpace(SSE_ACTIVATED))
        {
            System.Environment.Exit(-1);
        }
    }

    private static void InitTagVisualization()
    {
        TagFilter.SetInstance(new ConcreteTagFilter());
    }

    private static bool IsValidEnvironment()
    {
        try
        {
            string? javaHome = System.Environment.GetEnvironmentVariable("JAVA_HOME");
            if (!string.IsNullOrWhiteSpace(javaHome) && !javaHome.Contains("jdk", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("JAVA HOME is not properly aiming to the Java Development Kit");
            }

            Console.WriteLine($"Detected Java version is : {javaHome}");
        }
        catch (Exception)
        {
            Console.WriteLine("Exception: Something is wrong with your JAVA_HOME");
            Console.WriteLine("Check if JAVA_HOME system variable is correctly defined");
            Console.WriteLine("GO TO: https://testar.org/faq/ to obtain more details");
        }

        return true;
    }

    private static bool StartTestarDialog(Settings settings, string testSettingsFileName)
    {
        // Placeholder for UI dialog logic
        return true;
    }

    private static void InitCodingManager(Settings settings)
    {
        var stateManagementTags = StateManagementTags.getAllTags();
        if (stateManagementTags.Count > 0)
        {
            CodingManager.setCustomTagsForConcreteId(stateManagementTags.ToArray());
        }

        string abstractAttributes = settings.Get("AbstractStateAttributes", string.Empty);
        if (!string.IsNullOrWhiteSpace(abstractAttributes))
        {
            ITag[] abstractTags = abstractAttributes
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(tag => tag.Trim())
                .Select(StateManagementTags.getTagFromSettingsString)
                .Where(tag => tag != null)
                .Cast<ITag>()
                .ToArray();

            if (abstractTags.Length > 0)
            {
                CodingManager.setCustomTagsForAbstractId(abstractTags);
            }
        }
    }

    private static void InitOperatingSystem()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            org.testar.monkey.DefaultProtocol.WindowsAutomationProviderFactory = static () => new UIAAutomationProvider();
            Console.WriteLine("WARNING: No concrete environment implementation detected for Windows, using UnknownEnvironment.");
        }
        else
        {
            org.testar.monkey.DefaultProtocol.WindowsAutomationProviderFactory = null;
            Console.WriteLine("WARNING: Current OS has no concrete environment implementation, using UnknownEnvironment.");
        }

        TestarEnvironment.SetInstance(new UnknownEnvironment());
    }

    private static void StartTestar(Settings settings)
    {
        string protocolClassPath = settings.Get("ProtocolClass", "");
        if (string.IsNullOrWhiteSpace(protocolClassPath))
        {
            Console.WriteLine("ProtocolClass is not configured.");
            return;
        }

        string protocolClassName = protocolClassPath.Contains('/')
            ? protocolClassPath[(protocolClassPath.LastIndexOf('/') + 1)..]
            : protocolClassPath;
        string protocolFolder = protocolClassPath.Contains('/')
            ? protocolClassPath[..protocolClassPath.LastIndexOf('/')]
            : string.Empty;

        string protocolSourceDir = Path.Combine(settingsDir, protocolFolder);
        string compileDirSetting = settings.Get("ProtocolCompileDirectory", settingsDir);
        string compileDir = Path.IsPathRooted(compileDirSetting)
            ? compileDirSetting
            : Path.Combine(testarDir, compileDirSetting);
        string assemblyPath = Path.Combine(compileDir, protocolClassName + ".dll");

        try
        {
            bool alwaysCompile = bool.TryParse(settings.Get("AlwaysCompile", "false"), out bool compile) && compile;
            if (alwaysCompile || !File.Exists(assemblyPath))
            {
                ProtocolCompiler.Compile(protocolSourceDir, assemblyPath, protocolClassName);
            }

            Assembly protocolAssembly = Assembly.LoadFrom(assemblyPath);
            Type? protocolType = protocolAssembly.GetType(protocolClassName)
                ?? protocolAssembly.GetTypes().FirstOrDefault(t => t.Name == protocolClassName);
            if (protocolType == null)
            {
                Console.WriteLine($"Protocol type '{protocolClassName}' not found in {assemblyPath}");
                return;
            }

            object? protocolInstance = Activator.CreateInstance(protocolType);
            if (protocolInstance == null)
            {
                Console.WriteLine($"Failed to create protocol instance for {protocolClassName}");
                return;
            }

            MethodInfo? runMethod =
                protocolType.GetMethod("Run", new[] { typeof(org.testar.settings.Settings) }) ??
                protocolType.GetMethod("Run", new[] { typeof(Settings) }) ??
                protocolType.GetMethod("Run", Type.EmptyTypes) ??
                protocolType.GetMethod("run", new[] { typeof(org.testar.settings.Settings) }) ??
                protocolType.GetMethod("run", Type.EmptyTypes);

            if (runMethod == null)
            {
                Console.WriteLine($"No Run method found on protocol type {protocolClassName}");
                return;
            }

            if (runMethod.GetParameters().Length == 1)
            {
                var paramType = runMethod.GetParameters()[0].ParameterType;
                object argument = paramType == typeof(Settings)
                    ? settings
                    : ToTestarSettings(settings);
                runMethod.Invoke(protocolInstance, new object[] { argument });
            }
            else
            {
                runMethod.Invoke(protocolInstance, Array.Empty<object>());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading protocol: {ex.Message}");
        }
    }

    private static void StopTestar()
    {
        System.Environment.Exit(0);
    }

    private static bool ExistsSSE(string sseName)
    {
        return File.Exists(Path.Combine(settingsDir, sseName, SETTINGS_FILE));
    }

    private static string ExtractSSEName(string fileName)
    {
        if (!string.IsNullOrEmpty(fileName) && fileName.EndsWith(SUT_SETTINGS_EXT, StringComparison.OrdinalIgnoreCase))
        {
            return fileName[..^SUT_SETTINGS_EXT.Length];
        }

        return fileName;
    }

    private static void SettingsSelection()
    {
        if (sseManager == null)
        {
            SSE_ACTIVATED = null;
            return;
        }

        IReadOnlyList<string> availableSettings = sseManager.GetAvailableSettings();
        if (availableSettings.Count == 0)
        {
            Console.WriteLine("No SUT settings found!");
            SSE_ACTIVATED = null;
            return;
        }

        string? selected = Startup.SseSelectionDialog.SelectSse(availableSettings);
        if (string.IsNullOrWhiteSpace(selected))
        {
            SSE_ACTIVATED = null;
            return;
        }

        string sseFile = selected + SUT_SETTINGS_EXT;
        string sseFilePath = Path.Combine(settingsDir, sseFile);

        try
        {
            if (!File.Exists(sseFilePath))
            {
                using FileStream _ = File.Create(sseFilePath);
            }

            SSE_ACTIVATED = selected;
            return;
        }
        catch (IOException)
        {
            Console.WriteLine($"Exception creating <{sseFile}> file");
        }

        SSE_ACTIVATED = null;
    }

    private static org.testar.settings.Settings ToTestarSettings(Settings settings)
    {
        var testarSettings = new org.testar.settings.Settings();
        foreach (var kvp in settings.Properties)
        {
            testarSettings.Set(kvp.Key, kvp.Value);
        }

        return testarSettings;
    }
}
