using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Core.nTestar.Startup
{
    public sealed class SseManager
    {
        public string SettingsDir { get; }
        public string SettingsFileName { get; }
        public string SseExtension { get; }
        public string? ActiveSse { get; private set; }

        public SseManager(string settingsDir, string settingsFileName, string sseExtension)
        {
            if (string.IsNullOrWhiteSpace(settingsDir))
            {
                throw new ArgumentException("Settings directory is required.", nameof(settingsDir));
            }

            if (string.IsNullOrWhiteSpace(settingsFileName))
            {
                throw new ArgumentException("Settings file name is required.", nameof(settingsFileName));
            }

            if (string.IsNullOrWhiteSpace(sseExtension))
            {
                throw new ArgumentException("SSE extension is required.", nameof(sseExtension));
            }

            SettingsDir = settingsDir;
            SettingsFileName = settingsFileName;
            SseExtension = sseExtension.StartsWith(".", StringComparison.Ordinal) ? sseExtension : "." + sseExtension;
        }

        public string[] GetSseFiles()
        {
            if (!Directory.Exists(SettingsDir))
            {
                return Array.Empty<string>();
            }

            return Directory.GetFiles(SettingsDir, "*" + SseExtension)
                .Select(Path.GetFileName)
                .Where(name => !string.IsNullOrEmpty(name))
                .ToArray();
        }

        public IReadOnlyList<string> GetAvailableSettings()
        {
            if (!Directory.Exists(SettingsDir))
            {
                return Array.Empty<string>();
            }

            return Directory.GetDirectories(SettingsDir)
                .Where(dir => File.Exists(Path.Combine(dir, SettingsFileName)))
                .Select(Path.GetFileName)
                .Where(name => !string.IsNullOrEmpty(name))
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToArray();
        }

        public string GetTestSettingsFile()
        {
            if (string.IsNullOrWhiteSpace(ActiveSse))
            {
                throw new InvalidOperationException("Active SSE has not been selected.");
            }

            return Path.Combine(SettingsDir, ActiveSse, SettingsFileName);
        }

        public void InitSse(string[] args, Func<IReadOnlyList<string>, string?>? selectionFunc)
        {
            // Mirrors the original Java initTestarSSE flow for SSE selection and cleanup.
            if (args != null)
            {
                foreach (string arg in args)
                {
                    if (arg.Contains("sse=", StringComparison.Ordinal))
                    {
                        ProtocolFromCmd(arg);
                    }
                }
            }

            string[] files = GetSseFiles();
            if (files.Length > 1)
            {
                Console.WriteLine("Too many *.sse files - exactly one expected!");
                foreach (string file in files)
                {
                    string path = Path.Combine(SettingsDir, file);
                    Console.WriteLine($"Delete file <{file}> = {TryDelete(path)}");
                }

                files = Array.Empty<string>();
            }

            if (files.Length == 1)
            {
                string sseName = ExtractSseName(files[0]);
                if (!ExistsSse(sseName))
                {
                    Console.WriteLine("Protocol of indicated .sse file does not exist");
                    string path = Path.Combine(SettingsDir, files[0]);
                    Console.WriteLine($"Delete file <{files[0]}> = {TryDelete(path)}");
                    files = Array.Empty<string>();
                }
            }

            if (files.Length == 0)
            {
                if (selectionFunc == null)
                {
                    ActiveSse = null;
                    return;
                }

                if (!SelectAndCreateSse(selectionFunc))
                {
                    ActiveSse = null;
                    return;
                }
            }
            else
            {
                ActiveSse = ExtractSseName(files[0]);
            }
        }

        public void ProtocolFromCmd(string arg)
        {
            int index = arg.IndexOf('=', StringComparison.Ordinal);
            if (index < 0 || index == arg.Length - 1)
            {
                return;
            }

            string sseName = arg.Substring(index + 1);
            if (!ExistsSse(sseName))
            {
                Console.WriteLine($"CMD Protocol: {sseName} doesn't exist");
                return;
            }

            foreach (string file in GetSseFiles())
            {
                string path = Path.Combine(SettingsDir, file);
                TryDelete(path);
            }

            Directory.CreateDirectory(SettingsDir);
            string ssePath = Path.Combine(SettingsDir, sseName + SseExtension);
            if (!File.Exists(ssePath))
            {
                File.Create(ssePath).Close();
            }

            Console.WriteLine($"Protocol changed from command line to: {sseName}");
        }

        public bool ExistsSse(string sseName)
        {
            string settingsPath = Path.Combine(SettingsDir, sseName, SettingsFileName);
            return File.Exists(settingsPath);
        }

        private string ExtractSseName(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName) && fileName.EndsWith(SseExtension, StringComparison.OrdinalIgnoreCase))
            {
                return fileName.Substring(0, fileName.Length - SseExtension.Length);
            }

            return fileName;
        }

        private static bool TryDelete(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    return true;
                }
                return false;
            }
            catch (IOException)
            {
                return false;
            }
        }

        private bool SelectAndCreateSse(Func<IReadOnlyList<string>, string?> selectionFunc)
        {
            IReadOnlyList<string> available = GetAvailableSettings();
            if (available.Count == 0)
            {
                Console.WriteLine("No SUT settings found!");
                return false;
            }

            string? selected = selectionFunc(available);
            if (string.IsNullOrWhiteSpace(selected))
            {
                return false;
            }

            Directory.CreateDirectory(SettingsDir);
            string sseFilePath = Path.Combine(SettingsDir, selected + SseExtension);
            try
            {
                if (!File.Exists(sseFilePath))
                {
                    File.Create(sseFilePath).Close();
                }

                ActiveSse = selected;
                return true;
            }
            catch (IOException)
            {
                Console.WriteLine($"Exception creating <{selected + SseExtension}> file");
                return false;
            }
        }
    }
}
