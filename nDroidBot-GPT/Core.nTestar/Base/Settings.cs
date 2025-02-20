using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Reflection;

namespace Core.nTestar.Base
{
    public class Settings
    {
        private static string settingsPath;
        public static string SettingsPath
        {
            get => settingsPath;
            set => settingsPath = value;
        }

        public static Settings LoadSettings(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                throw new ArgumentException("Invalid settings file path");
            }

            var settings = new Settings();
            var properties = File.ReadAllLines(filePath)
                .Where(line => !string.IsNullOrWhiteSpace(line) && line.Contains("="))
                .Select(line => line.Split(new[] { '=' }, 2))
                .ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim());

            settings.Properties = properties;
            return settings;
        }

        public Dictionary<string, string> Properties { get; private set; } = new Dictionary<string, string>();

        public string Get(string key, string defaultValue = "")
        {
            return Properties.TryGetValue(key, out var value) ? value : defaultValue;
        }

        public void Set(string key, string value)
        {
            Properties[key] = value;
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
    }

}
