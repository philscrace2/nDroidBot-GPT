using System;
using System.IO;
using Core.nTestar.Base;
using Core.nTestar.Startup;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace nDroiBot_GPT_Tests
{
    [TestFixture]
    public class TestarStartup_Tests
    {
        [Test]
        public void LoadSettings_ParsesJavaPropertiesAndOverrides()
        {
            string root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(root);
            string settingsPath = Path.Combine(root, "test.settings");

            string content =
                "# Comment line\n" +
                "! Another comment\n" +
                "key1=value1\n" +
                "key2: value2\n" +
                "key3 value3\n" +
                "escaped=hello\\ world\n" +
                "multi=first\\\n" +
                " second\n" +
                "unicode=\\u0041\n" +
                "tab=hello\\tthere\n";

            File.WriteAllText(settingsPath, content);

            Settings settings = Settings.LoadSettings(new[] { "key1=override", "sse=ignored" }, settingsPath);

            Assert.That(settings.Get("key1"), Is.EqualTo("override"));
            Assert.That(settings.Get("key2"), Is.EqualTo("value2"));
            Assert.That(settings.Get("key3"), Is.EqualTo("value3"));
            Assert.That(settings.Get("escaped"), Is.EqualTo("hello world"));
            Assert.That(settings.Get("multi"), Is.EqualTo("firstsecond"));
            Assert.That(settings.Get("unicode"), Is.EqualTo("A"));
            Assert.That(settings.Get("tab"), Is.EqualTo("hello\tthere"));
        }

        [Test]
        public void InitSse_RemovesExtraSseFiles()
        {
            string root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            string settingsDir = Path.Combine(root, "settings");
            Directory.CreateDirectory(settingsDir);

            CreateSutSettings(settingsDir, "alpha");
            CreateSutSettings(settingsDir, "beta");
            File.WriteAllText(Path.Combine(settingsDir, "alpha.sse"), string.Empty);
            File.WriteAllText(Path.Combine(settingsDir, "beta.sse"), string.Empty);

            var manager = new SseManager(settingsDir, "test.settings", ".sse");
            manager.InitSse(Array.Empty<string>(), _ => null);

            Assert.That(manager.GetSseFiles(), Is.Empty);
            Assert.That(manager.ActiveSse, Is.Null);
        }

        [Test]
        public void ProtocolFromCmd_ReplacesExistingSseFile()
        {
            string root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            string settingsDir = Path.Combine(root, "settings");
            Directory.CreateDirectory(settingsDir);

            CreateSutSettings(settingsDir, "alpha");
            File.WriteAllText(Path.Combine(settingsDir, "old.sse"), string.Empty);

            var manager = new SseManager(settingsDir, "test.settings", ".sse");
            manager.ProtocolFromCmd("sse=alpha");

            Assert.That(manager.GetSseFiles(), Is.EquivalentTo(new[] { "alpha.sse" }));
        }

        private static void CreateSutSettings(string settingsDir, string name)
        {
            string sutDir = Path.Combine(settingsDir, name);
            Directory.CreateDirectory(sutDir);
            File.WriteAllText(Path.Combine(sutDir, "test.settings"), "Mode=Generate");
        }
    }
}
