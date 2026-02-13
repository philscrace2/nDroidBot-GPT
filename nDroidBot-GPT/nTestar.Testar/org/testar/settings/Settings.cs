using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using org.testar.monkey;

namespace org.testar.settings
{
    public class Settings
    {
        public const string SUT_CONNECTOR_COMMAND_LINE = "COMMAND_LINE";
        public const string SUT_CONNECTOR_WINDOW_TITLE = "SUT_WINDOW_TITLE";
        public const string SUT_CONNECTOR_PROCESS_NAME = "SUT_PROCESS_NAME";

        private static string? settingsPath;
        public static string? SettingsPath
        {
            get => settingsPath;
            set => settingsPath = value;
        }

        public static Settings LoadSettings(string filePath)
        {
            return LoadSettings(Array.Empty<string>(), filePath);
        }

        public static Settings LoadSettings(string[] args, string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                throw new ArgumentException("Invalid settings file path");
            }

            var settings = new Settings();
            var properties = ParseProperties(filePath);

            if (args != null && args.Length > 0)
            {
                foreach (string arg in args)
                {
                    if (arg.Contains("sse=", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    foreach (var kvp in ParsePropertiesFromString(arg))
                    {
                        properties[kvp.Key] = kvp.Value;
                    }
                }
            }

            settings.Properties = properties;
            return settings;
        }

        public Dictionary<string, string> Properties { get; private set; } = new Dictionary<string, string>();

        public string Get(string key, string defaultValue = "")
        {
            return Properties.TryGetValue(key, out var value) ? value : defaultValue;
        }

        public string get(string key, string defaultValue = "")
        {
            return Get(key, defaultValue);
        }

        public T Get<T>(org.testar.monkey.alayer.Tag<T> tag, T defaultValue)
        {
            if (!Properties.TryGetValue(tag.name(), out var value))
            {
                return defaultValue;
            }

            object? converted = ConvertToType(value, typeof(T));
            return converted is T typed ? typed : defaultValue;
        }

        public T get<T>(org.testar.monkey.alayer.Tag<T> tag)
        {
            return Get(tag, default!);
        }

        public T get<T>(org.testar.monkey.alayer.Tag<T> tag, T defaultValue)
        {
            return Get(tag, defaultValue);
        }

        public void Set<T>(org.testar.monkey.alayer.Tag<T> tag, T value)
        {
            Properties[tag.name()] = value?.ToString() ?? string.Empty;
        }

        public void set<T>(org.testar.monkey.alayer.Tag<T> tag, T value)
        {
            Set(tag, value);
        }

        public void Set(string key, string value)
        {
            Properties[key] = value;
        }

        public void set(string key, string value)
        {
            Set(key, value);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var kvp in Properties)
            {
                sb.AppendLine($"{kvp.Key} = {kvp.Value}");
            }
            return sb.ToString();
        }

        private static Dictionary<string, string> ParseProperties(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            return ParsePropertiesFromLines(lines);
        }

        private static Dictionary<string, string> ParsePropertiesFromLines(IEnumerable<string> lines)
        {
            var properties = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#", StringComparison.Ordinal))
                {
                    continue;
                }

                int separatorIndex = line.IndexOf('=');
                if (separatorIndex <= 0)
                {
                    continue;
                }

                string key = line.Substring(0, separatorIndex).Trim();
                string value = line.Substring(separatorIndex + 1).Trim();
                properties[key] = value;
            }

            return properties;
        }

        private static Dictionary<string, string> ParsePropertiesFromString(string input)
        {
            var properties = new Dictionary<string, string>();
            var parts = input.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                int separatorIndex = part.IndexOf('=');
                if (separatorIndex <= 0)
                {
                    continue;
                }

                string key = part.Substring(0, separatorIndex).Trim();
                string value = part.Substring(separatorIndex + 1).Trim();
                properties[key] = value;
            }

            return properties;
        }

        private static object? ConvertToType(string value, Type targetType)
        {
            if (targetType == typeof(string))
            {
                return value;
            }

            if (targetType == typeof(bool) && bool.TryParse(value, out var boolValue))
            {
                return boolValue;
            }

            if (targetType == typeof(int) && int.TryParse(value, out var intValue))
            {
                return intValue;
            }

            if (targetType == typeof(float) && float.TryParse(value, out var floatValue))
            {
                return floatValue;
            }

            if (targetType == typeof(double) && double.TryParse(value, out var doubleValue))
            {
                return doubleValue;
            }

            if (targetType == typeof(List<string>))
            {
                var list = value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(item => item.Trim())
                    .Where(item => item.Length > 0)
                    .ToList();
                return list;
            }

            if (targetType == typeof(List<Pair<string, string>>))
            {
                var pairs = new List<Pair<string, string>>();
                foreach (string entry in value.Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] parts = entry.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        pairs.Add(Pair<string, string>.from(parts[0], parts[1]));
                    }
                }

                return pairs;
            }

            return null;
        }
    }
}
