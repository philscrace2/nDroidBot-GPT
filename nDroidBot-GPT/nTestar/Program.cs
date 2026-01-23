// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Core.nTestar.Base;
using Core.nTestar.Settings.Dialog.TagsVisualization;


public class MainClass
{
    public const string TESTAR_VERSION = "2.6.20 (14-Aug-2024)";
    public const string SETTINGS_FILE = "test.settings";
    public const string SUT_SETTINGS_EXT = ".sse";
    public static string SSE_ACTIVATED = null;

    public static string testarDir = "." + Path.DirectorySeparatorChar;
    public static string settingsDir = testarDir + "settings" + Path.DirectorySeparatorChar;
    public static string outputDir = testarDir + "output" + Path.DirectorySeparatorChar;
    public static string tempDir = outputDir + "temp" + Path.DirectorySeparatorChar;

    public static string[] GetSSE()
    {
        return Directory.Exists(settingsDir) ?
            Directory.GetFiles(settingsDir, "*" + SUT_SETTINGS_EXT)
                .Select(Path.GetFileName)
                .ToArray()
            : new string[0];
    }

    public static string GetTestSettingsFile()
    {
        return Path.Combine(settingsDir, SSE_ACTIVATED, SETTINGS_FILE);
    }

    public static void Main(string[] args)
    {
        VerifyTestarInitialDirectory();
        InitTagVisualization();
        InitTestarSSE(args);

        string testSettingsFileName = GetTestSettingsFile();
        Console.WriteLine($"TESTAR version is <{TESTAR_VERSION}>");
        Console.WriteLine($"Test settings file: <{testSettingsFileName}>");

        Settings settings = Settings.LoadSettings(testSettingsFileName);

        if (settings.Get("ShowVisualSettingsDialogOnStartup", "false") != null)
        {
            SetTestarDirectory(settings);
            StartTestar(settings);
        }
        else
        {
            while (StartTestarDialog(settings, testSettingsFileName))
            {
                testSettingsFileName = GetTestSettingsFile();
                settings = Settings.LoadSettings(testSettingsFileName);
                SetTestarDirectory(settings);
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
            Environment.Exit(-1);
        }
    }

    private static void SetTestarDirectory(Settings settings)
    {
        outputDir = settings.Get("OutputDir", outputDir);
        tempDir = settings.Get("TempDir", tempDir);
    }

    private static void InitTestarSSE(string[] args)
    {
        string[] files = GetSSE();

        if (files.Length > 1)
        {
            Console.WriteLine("Too many .sse files - exactly one expected!");
            foreach (string file in files)
                File.Delete(Path.Combine(settingsDir, file));
            files = new string[0];
        }

        if (files.Length == 1 && !ExistsSSE(Path.GetFileNameWithoutExtension(files[0])))
        {
            Console.WriteLine("Protocol of indicated .sse file does not exist");
            File.Delete(Path.Combine(settingsDir, files[0]));
            files = new string[0];
        }

        if (files.Length == 0)
        {
            SettingsSelection();
            if (SSE_ACTIVATED == null)
                Environment.Exit(-1);
        }
        else
        {
            SSE_ACTIVATED = Path.GetFileNameWithoutExtension(files[0]);
        }
    }

    private static void InitTagVisualization()
    {
        TagFilter.SetInstance(new ConcreteTagFilter());
    }

    private static void SettingsSelection()
    {
        var sutSettings = Directory.GetDirectories(settingsDir)
            .Where(dir => File.Exists(Path.Combine(dir, SETTINGS_FILE)))
            .Select(Path.GetFileName)
            .ToList();

        if (!sutSettings.Any())
        {
            Console.WriteLine("No SUT settings found!");
            return;
        }

        string sseSelected = ShowSelectionDialog("Select the desired setting:", "TESTAR settings", sutSettings);
        if (sseSelected == null)
        {
            SSE_ACTIVATED = null;
            return;
        }

        string sseFile = sseSelected + SUT_SETTINGS_EXT;
        File.Create(Path.Combine(settingsDir, sseFile)).Close();
        SSE_ACTIVATED = sseSelected;
    }

    private static string ShowSelectionDialog(string prompt, string title, List<string> options)
    {
        using (Form form = new Form())
        {
            ComboBox comboBox = new ComboBox() { DataSource = options, Dock = DockStyle.Fill };
            Button okButton = new Button() { Text = "OK", Dock = DockStyle.Bottom };
            okButton.Click += (s, e) => form.Close();

            form.Controls.Add(comboBox);
            form.Controls.Add(okButton);
            form.Text = title;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.ShowDialog();

            return comboBox.SelectedItem?.ToString();
        }
    }

    private static bool StartTestarDialog(Settings settings, string testSettingsFileName)
    {
        // Placeholder for UI dialog logic
        return true;
    }

    private static void StartTestar(Settings settings)
    {
        string protocolClassPath = settings.Get("ProtocolClass", "");
        string assemblyPath = Path.Combine(settings.Get("ProtocolCompileDirectory", ""), protocolClassPath);

        try
        {
            Assembly protocolAssembly = Assembly.LoadFrom(assemblyPath);
            Type protocolType = protocolAssembly.GetType(protocolClassPath);
            object protocolInstance = Activator.CreateInstance(protocolType);
            MethodInfo runMethod = protocolType.GetMethod("Run");
            runMethod.Invoke(protocolInstance, new object[] { settings });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading protocol: {ex.Message}");
        }
    }

    private static void StopTestar()
    {
        Environment.Exit(0);
    }

    private static bool ExistsSSE(string sseName)
    {
        return File.Exists(Path.Combine(settingsDir, sseName, SETTINGS_FILE));
    }
}
