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
            return LoadSettings(Array.Empty<string>(), filePath);
        }

        public static Settings LoadSettings(string[] args, string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
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

        private static Dictionary<string, string> ParseProperties(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            return ParsePropertiesFromLines(lines);
        }

        private static Dictionary<string, string> ParsePropertiesFromString(string propertiesLine)
        {
            return ParsePropertiesFromLines(new[] { propertiesLine });
        }

        private static Dictionary<string, string> ParsePropertiesFromLines(IEnumerable<string> lines)
        {
            var properties = new Dictionary<string, string>();

            foreach (string logicalLine in ReadLogicalLines(lines))
            {
                string line = logicalLine.TrimStart();
                if (line.Length == 0)
                {
                    continue;
                }

                if (line.StartsWith("#", StringComparison.Ordinal) || line.StartsWith("!", StringComparison.Ordinal))
                {
                    continue;
                }

                int separatorIndex = FindSeparatorIndex(line, out int keyLength);
                string keyRaw;
                string valueRaw;

                if (separatorIndex < 0)
                {
                    keyRaw = line;
                    valueRaw = string.Empty;
                }
                else
                {
                    keyRaw = line.Substring(0, keyLength);
                    int valueStart = separatorIndex;

                    if (char.IsWhiteSpace(line[separatorIndex]))
                    {
                        while (valueStart < line.Length && char.IsWhiteSpace(line[valueStart]))
                        {
                            valueStart++;
                        }

                        if (valueStart < line.Length && (line[valueStart] == '=' || line[valueStart] == ':'))
                        {
                            valueStart++;
                        }
                    }
                    else
                    {
                        valueStart++;
                    }

                    while (valueStart < line.Length && char.IsWhiteSpace(line[valueStart]))
                    {
                        valueStart++;
                    }

                    valueRaw = valueStart >= line.Length ? string.Empty : line.Substring(valueStart);
                }

                string key = Unescape(keyRaw.Trim());
                string value = Unescape(valueRaw.Trim());

                if (key.Length > 0)
                {
                    properties[key] = value;
                }
            }

            return properties;
        }

        private static IEnumerable<string> ReadLogicalLines(IEnumerable<string> lines)
        {
            StringBuilder? current = null;
            bool continuing = false;

            foreach (string rawLine in lines)
            {
                string line = continuing ? rawLine.TrimStart() : rawLine;

                if (current == null)
                {
                    current = new StringBuilder(line);
                }
                else
                {
                    current.Append(line);
                }

                if (EndsWithContinuation(current.ToString()))
                {
                    current.Length -= 1;
                    continuing = true;
                }
                else
                {
                    continuing = false;
                    yield return current.ToString();
                    current = null;
                }
            }

            if (current != null)
            {
                yield return current.ToString();
            }
        }

        private static bool EndsWithContinuation(string line)
        {
            int count = 0;
            for (int i = line.Length - 1; i >= 0 && line[i] == '\\'; i--)
            {
                count++;
            }

            return count % 2 == 1;
        }

        private static int FindSeparatorIndex(string line, out int keyLength)
        {
            bool escaped = false;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (!escaped)
                {
                    if (c == '=' || c == ':')
                    {
                        keyLength = i;
                        return i;
                    }

                    if (char.IsWhiteSpace(c))
                    {
                        keyLength = i;
                        return i;
                    }
                }

                escaped = c == '\\' && !escaped;
            }

            keyLength = line.Length;
            return -1;
        }

        private static string Unescape(string input)
        {
            if (input.IndexOf('\\') < 0)
            {
                return input;
            }

            var sb = new StringBuilder(input.Length);
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (c != '\\')
                {
                    sb.Append(c);
                    continue;
                }

                if (i == input.Length - 1)
                {
                    sb.Append('\\');
                    break;
                }

                char next = input[++i];
                switch (next)
                {
                    case 't':
                        sb.Append('\t');
                        break;
                    case 'n':
                        sb.Append('\n');
                        break;
                    case 'r':
                        sb.Append('\r');
                        break;
                    case 'f':
                        sb.Append('\f');
                        break;
                    case 'u':
                        if (i + 4 <= input.Length - 1)
                        {
                            string hex = input.Substring(i + 1, 4);
                            if (int.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out int code))
                            {
                                sb.Append((char)code);
                                i += 4;
                            }
                            else
                            {
                                sb.Append('u');
                            }
                        }
                        else
                        {
                            sb.Append('u');
                        }
                        break;
                    default:
                        sb.Append(next);
                        break;
                }
            }

            return sb.ToString();
        }
    }

}
