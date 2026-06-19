using System.Globalization;

namespace nTestar.Desktop.Winforms.mvp;

internal sealed class TestarGeneralSettingsSource
{
    private const string SettingsFileName = "test.testarsettings";
    private const string SseExtension = ".sse";

    public MainScreenModel LoadOrDefault(MainScreenModel fallback)
    {
        string? root = FindSolutionRoot(AppContext.BaseDirectory);
        if (string.IsNullOrWhiteSpace(root))
        {
            return fallback;
        }

        string settingsRoot = Path.Combine(root, "nTestar", "settings");
        if (!Directory.Exists(settingsRoot))
        {
            return fallback;
        }

        List<string> protocols = Directory.GetDirectories(settingsRoot)
            .Where(d => File.Exists(Path.Combine(d, SettingsFileName)))
            .Select(Path.GetFileName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Cast<string>()
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (protocols.Count == 0)
        {
            return fallback;
        }

        string selectedProtocol = ResolveSelectedProtocol(settingsRoot, protocols);
        Dictionary<string, string> values = ParseSettings(Path.Combine(settingsRoot, selectedProtocol, SettingsFileName));

        return new MainScreenModel
        {
            SutConnector = Get(values, "SUTConnectorValue", fallback.SutConnector),
            SutConnectorType = Get(values, "SUTConnector", fallback.SutConnectorType),
            NumberOfSequences = GetInt(values, "Sequences", fallback.NumberOfSequences),
            SequenceActions = GetInt(values, "SequenceLength", fallback.SequenceActions),
            AlwaysCompileProtocol = GetBool(values, "AlwaysCompile", fallback.AlwaysCompileProtocol),
            Protocol = ResolveProtocolFromClass(values, selectedProtocol),
            ApplicationName = Get(values, "ApplicationName", fallback.ApplicationName),
            ApplicationVersion = Get(values, "ApplicationVersion", fallback.ApplicationVersion),
            OverrideDisplayScale = Get(values, "OverrideWebDriverDisplayScale", fallback.OverrideDisplayScale),
            VisualizeActionsOnGui = GetBool(values, "VisualizeActions", fallback.VisualizeActionsOnGui),
            Protocols = protocols
        };
    }

    public void SaveGeneralSettings(string protocol, IMainView view, string? mode = null)
    {
        string? root = FindSolutionRoot(AppContext.BaseDirectory);
        if (string.IsNullOrWhiteSpace(root) || string.IsNullOrWhiteSpace(protocol))
        {
            return;
        }

        string settingsFile = Path.Combine(root, "nTestar", "settings", protocol, SettingsFileName);
        if (!File.Exists(settingsFile))
        {
            return;
        }

        var updates = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["SUTConnector"] = view.SutConnectorType,
            ["SUTConnectorValue"] = view.SutConnector,
            ["Sequences"] = view.NumberOfSequences.ToString(CultureInfo.InvariantCulture),
            ["SequenceLength"] = view.SequenceActions.ToString(CultureInfo.InvariantCulture),
            ["AlwaysCompile"] = view.AlwaysCompileProtocol ? "true" : "false",
            ["ApplicationName"] = view.ApplicationName,
            ["ApplicationVersion"] = view.ApplicationVersion
        };
        if (!string.IsNullOrWhiteSpace(mode))
        {
            updates["Mode"] = mode;
        }

        var lines = File.ReadAllLines(settingsFile).ToList();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < lines.Count; i++)
        {
            string trimmed = lines[i].Trim();
            if (trimmed.Length == 0 || trimmed.StartsWith("#", StringComparison.Ordinal))
            {
                continue;
            }

            int separator = lines[i].IndexOf('=');
            if (separator < 1)
            {
                continue;
            }

            string key = lines[i][..separator].Trim();
            if (!updates.TryGetValue(key, out string? value))
            {
                continue;
            }

            lines[i] = $"{key} = {value}";
            seen.Add(key);
        }

        foreach ((string key, string value) in updates)
        {
            if (!seen.Contains(key))
            {
                lines.Add($"{key} = {value}");
            }
        }

        File.WriteAllLines(settingsFile, lines);
    }

    private static string ResolveSelectedProtocol(string settingsRoot, List<string> protocols)
    {
        string? sseFile = Directory.GetFiles(settingsRoot, "*" + SseExtension)
            .Select(Path.GetFileName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(sseFile))
        {
            string candidate = sseFile[..^SseExtension.Length];
            if (protocols.Contains(candidate, StringComparer.OrdinalIgnoreCase))
            {
                return protocols.First(p => p.Equals(candidate, StringComparison.OrdinalIgnoreCase));
            }
        }

        return protocols.Contains("desktop_generic", StringComparer.OrdinalIgnoreCase)
            ? protocols.First(p => p.Equals("desktop_generic", StringComparison.OrdinalIgnoreCase))
            : protocols[0];
    }

    private static string ResolveProtocolFromClass(Dictionary<string, string> values, string fallback)
    {
        string protocolClass = Get(values, "ProtocolClass", string.Empty);
        if (!string.IsNullOrWhiteSpace(protocolClass))
        {
            string[] split = protocolClass.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (split.Length > 0)
            {
                return split[0];
            }
        }

        return fallback;
    }

    private static Dictionary<string, string> ParseSettings(string settingsFile)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (!File.Exists(settingsFile))
        {
            return map;
        }

        foreach (string rawLine in File.ReadLines(settingsFile))
        {
            string line = rawLine.Trim();
            if (line.Length == 0 || line.StartsWith("#", StringComparison.Ordinal))
            {
                continue;
            }

            int separator = line.IndexOf('=');
            if (separator < 1)
            {
                continue;
            }

            string key = line[..separator].Trim();
            string value = line[(separator + 1)..].Trim();
            map[key] = value;
        }

        return map;
    }

    private static string Get(Dictionary<string, string> values, string key, string fallback)
        => values.TryGetValue(key, out string? value) && !string.IsNullOrWhiteSpace(value) ? value : fallback;

    private static int GetInt(Dictionary<string, string> values, string key, int fallback)
        => values.TryGetValue(key, out string? value) && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed)
            ? parsed
            : fallback;

    private static bool GetBool(Dictionary<string, string> values, string key, bool fallback)
        => values.TryGetValue(key, out string? value) && bool.TryParse(value, out bool parsed)
            ? parsed
            : fallback;

    private static string? FindSolutionRoot(string startDirectory)
    {
        var directory = new DirectoryInfo(startDirectory);
        while (directory != null)
        {
            string sln = Path.Combine(directory.FullName, "nDroidBot-GPT.sln");
            if (File.Exists(sln))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return null;
    }
}
