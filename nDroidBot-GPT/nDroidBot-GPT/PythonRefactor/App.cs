using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
//using AndroidAppParser; // This is a placeholder for the actual APK parsing library, such as Androguard

namespace nDroidBot_GPT.PythonRefactor
{
    public class App
    {
        private readonly string appPath;
        private readonly string outputDir;
        private readonly APK apk;
        private string packageName;
        private string appName;
        private string mainActivity;
        private List<string> permissions;
        private List<string> activities;
        private List<Intent> possibleBroadcasts;
        private string dumpsysMainActivity;
        private List<string> hashes;

        public App(string appPath, string outputDir = null)
        {
            if (appPath == null) throw new ArgumentNullException(nameof(appPath));

            this.appPath = appPath;
            this.outputDir = outputDir;

            if (outputDir != null && !Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            // Initialize APK object
            this.apk = new APK(appPath);

            this.packageName = this.apk.GetPackage();
            this.appName = this.apk.GetAppName();
            this.mainActivity = this.apk.GetMainActivity();
            this.permissions = this.apk.GetPermissions();
            this.activities = this.apk.GetActivities();
            this.possibleBroadcasts = GetPossibleBroadcasts();
            this.dumpsysMainActivity = null;
            this.hashes = GetHashes();
        }

        public string GetPackageName()
        {
            return this.packageName;
        }

        public string GetMainActivity()
        {
            if (this.mainActivity != null)
            {
                return this.mainActivity;
            }
            else
            {
                Console.WriteLine("Cannot get main activity from manifest. Using dumpsys result instead.");
                return this.dumpsysMainActivity;
            }
        }

        public Intent GetStartIntent()
        {
            var packageName = GetPackageName();
            if (GetMainActivity() != null)
            {
                packageName += "/" + GetMainActivity();
            }
            return new Intent(packageName);
        }

        public Intent GetStartWithProfilingIntent(string traceFile, int? sampling = null)
        {
            var packageName = GetPackageName();
            if (GetMainActivity() != null)
            {
                packageName += "/" + GetMainActivity();
            }

            if (sampling.HasValue)
            {
                return new Intent($"start --start-profiler {traceFile} --sampling {sampling.Value}", packageName);
            }
            else
            {
                return new Intent($"start --start-profiler {traceFile}", packageName);
            }
        }

        public Intent GetStopIntent()
        {
            var packageName = GetPackageName();
            return new Intent("force-stop", packageName);
        }

        public List<Intent> GetPossibleBroadcasts()
        {
            var possibleBroadcasts = new HashSet<Intent>();
            foreach (var receiver in this.apk.GetReceivers())
            {
                var intentFilters = this.apk.GetIntentFilters("receiver", receiver);
                var actions = intentFilters.ContainsKey("action") ? intentFilters["action"] : new List<string>();
                var categories = intentFilters.ContainsKey("category") ? intentFilters["category"] : new List<string>();
                categories.Add(null);

                foreach (var action in actions)
                {
                    foreach (var category in categories)
                    {
                        var intent = new Intent("broadcast", action, category);
                        possibleBroadcasts.Add(intent);
                    }
                }
            }
            return new List<Intent>(possibleBroadcasts);
        }

        public List<string> GetHashes(int blockSize = 256)
        {
            using (var md5 = MD5.Create())
            using (var sha1 = SHA1.Create())
            using (var sha256 = SHA256.Create())
            using (var stream = new FileStream(this.appPath, FileMode.Open, FileAccess.Read))
            {
                var buffer = new byte[blockSize];
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    md5.TransformBlock(buffer, 0, bytesRead, null, 0);
                    sha1.TransformBlock(buffer, 0, bytesRead, null, 0);
                    sha256.TransformBlock(buffer, 0, bytesRead, null, 0);
                }

                md5.TransformFinalBlock(buffer, 0, 0);
                sha1.TransformFinalBlock(buffer, 0, 0);
                sha256.TransformFinalBlock(buffer, 0, 0);

                return new List<string>
                {
                    BitConverter.ToString(md5.Hash).Replace("-", "").ToLower(),
                    BitConverter.ToString(sha1.Hash).Replace("-", "").ToLower(),
                    BitConverter.ToString(sha256.Hash).Replace("-", "").ToLower()
                };
            }
        }
    }
}
