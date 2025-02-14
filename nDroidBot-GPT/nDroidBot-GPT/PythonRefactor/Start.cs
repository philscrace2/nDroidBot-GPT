using CommandLine;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace nDroidBot_GPT.PythonRefactor
{
    class Program
    {
        public class Options
        {
            [Option('d', "device_serial", HelpText = "The serial number of target device (use `adb devices` to find)", Required = false)]
            public string DeviceSerial { get; set; }

            [Option('a', "apk_path", HelpText = "The file path to target APK", Required = true)]
            public string ApkPath { get; set; }

            [Option('o', "output_dir", HelpText = "Directory of output")]
            public string OutputDir { get; set; }

            [Option("policy", HelpText = "Policy to use for test input generation.", Default = "default_policy")]
            public string InputPolicy { get; set; }

            [Option("distributed", HelpText = "Start DroidBot in distributed mode.", Default = "normal")]
            public string Distributed { get; set; }

            [Option("master", HelpText = "DroidMaster's RPC address")]
            public string Master { get; set; }

            [Option("qemu_hda", HelpText = "The QEMU's hda image")]
            public string QemuHda { get; set; }

            [Option("qemu_no_graphic", HelpText = "Run QEMU with -nograph parameter", Default = false)]
            public bool QemuNoGraphic { get; set; }

            [Option("script", HelpText = "Use a script to customize input for certain states.")]
            public string ScriptPath { get; set; }

            [Option("count", HelpText = "Number of events to generate in total. Default: 10", Default = 10)]
            public int Count { get; set; }

            [Option("interval", HelpText = "Interval in seconds between events. Default: 1", Default = 1)]
            public int Interval { get; set; }

            [Option("timeout", HelpText = "Timeout in seconds. Default: 30", Default = 30)]
            public int Timeout { get; set; }

            [Option("cv", HelpText = "Use OpenCV to identify UI components", Default = false)]
            public bool CvMode { get; set; }

            [Option("debug", HelpText = "Run in debug mode", Default = false)]
            public bool DebugMode { get; set; }

            [Option("random", HelpText = "Add randomness to input events", Default = false)]
            public bool RandomInput { get; set; }

            [Option("keep_app", HelpText = "Keep the app on the device after testing", Default = false)]
            public bool KeepApp { get; set; }

            [Option("grant_perm", HelpText = "Grant all permissions while installing", Default = false)]
            public bool GrantPerm { get; set; }

            [Option("is_emulator", HelpText = "Declare the target device to be an emulator", Default = false)]
            public bool IsEmulator { get; set; }

            [Option("accessibility_auto", HelpText = "Enable accessibility service automatically", Default = false)]
            public bool EnableAccessibilityHard { get; set; }

            [Option("humanoid", HelpText = "Connect to a Humanoid service (addr:port) for more human-like behaviors.")]
            public string Humanoid { get; set; }

            [Option("ignore_ad", HelpText = "Ignore Ad views by checking resource_id", Default = false)]
            public bool IgnoreAd { get; set; }

            [Option("replay_output", HelpText = "The droidbot output directory being replayed.")]
            public string ReplayOutput { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(opts =>
                {
                    if (!File.Exists(opts.ApkPath))
                    {
                        Console.WriteLine("APK does not exist.");
                        return;
                    }

                    if (opts.CvMode && string.IsNullOrEmpty(opts.OutputDir))
                    {
                        Console.WriteLine("To run in CV mode, you need to specify an output dir (using -o option).");
                        return;
                    }

                    string startMode = opts.Distributed.ToLower() switch
                    {
                        "master" => "master",
                        "worker" => "worker",
                        _ => "normal"
                    };

                    if (startMode == "master")
                    {
                        // Start DroidMaster
                        DroidMaster droidmaster = new DroidMaster(
                            opts.ApkPath,
                            opts.IsEmulator,
                            opts.OutputDir,
                            opts.InputPolicy,
                            opts.RandomInput,
                            opts.ScriptPath,
                            opts.Interval,
                            opts.Timeout,
                            opts.Count,
                            opts.CvMode,
                            opts.DebugMode,
                            opts.KeepApp,
                            opts.GrantPerm,
                            opts.EnableAccessibilityHard,
                            opts.QemuHda,
                            opts.QemuNoGraphic,
                            opts.Humanoid,
                            opts.IgnoreAd,
                            opts.ReplayOutput
                        );
                        droidmaster.Start();
                    }
                    else
                    {
                        // Start DroidBot
                        DroidBot droidbot = new DroidBot(
                            opts.ApkPath,
                            opts.DeviceSerial,

                            opts.IsEmulator,
                            opts.OutputDir,
                            opts.InputPolicy,
                            opts.RandomInput,
                            opts.ScriptPath,
                            opts.Interval,
                            opts.Timeout,
                            opts.Count,
                            opts.CvMode,
                            opts.DebugMode,
                            opts.KeepApp,
                            opts.GrantPerm,
                            opts.EnableAccessibilityHard,
                            opts.Master,
                            opts.Humanoid,
                            opts.IgnoreAd,
                            opts.ReplayOutput
                        );
                        droidbot.Start();
                    }
                });
        }
    }

    // DroidMaster and DroidBot classes need to be implemented here.
}
