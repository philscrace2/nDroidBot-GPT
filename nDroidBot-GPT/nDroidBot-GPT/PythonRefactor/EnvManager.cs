using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace nDroidBot_GPT.PythonRefactor
{
    public class UnknownEnvException : Exception
    {
    }

    public abstract class AppEnv
    {
        public abstract void Deploy(Device device);

        public Dictionary<string, object> ToDictionary()
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var prop in this.GetType().GetProperties())
            {
                dictionary[prop.Name] = prop.GetValue(this);
            }
            return dictionary;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(ToDictionary());
        }

        public override string ToString()
        {
            return ToDictionary().ToString();
        }
    }

    public abstract class StaticAppEnv : AppEnv
    {
        public abstract override void Deploy(Device device);
    }

    public abstract class DynamicAppEnv : AppEnv
    {
        public abstract override void Deploy(Device device);
    }

    public class ContactAppEnv : StaticAppEnv
    {
        public string Name { get; set; } = "Lynn";
        public string Phone { get; set; } = "1234567890";
        public string Email { get; set; } = "droidbot@honeynet.com";
        public string EnvType { get; set; } = "contact";

        public ContactAppEnv() { }

        public ContactAppEnv(Dictionary<string, object> envDict)
        {
            foreach (var pair in envDict)
            {
                GetType().GetProperty(pair.Key)?.SetValue(this, pair.Value);
            }
        }

        public override void Deploy(Device device)
        {
            var contactData = ToDictionary();
            contactData.Remove("EnvType");
            device.AddContact(contactData);
        }
    }

    public class SettingsAppEnv : StaticAppEnv
    {
        public string TableName { get; set; } = "system";
        public string Name { get; set; } = "screen_brightness";
        public string Value { get; set; } = "50";
        public string EnvType { get; set; } = "settings";

        public SettingsAppEnv() { }

        public SettingsAppEnv(Dictionary<string, object> envDict)
        {
            foreach (var pair in envDict)
            {
                GetType().GetProperty(pair.Key)?.SetValue(this, pair.Value);
            }
        }

        public override void Deploy(Device device)
        {
            device.ChangeSettings(TableName, Name, Value);
        }
    }

    public class CallLogEnv : StaticAppEnv
    {
        public string Phone { get; set; } = "1234567890";
        public bool CallIn { get; set; } = true;
        public bool Accepted { get; set; } = true;
        public string EnvType { get; set; } = "calllog";

        public CallLogEnv() { }

        public CallLogEnv(Dictionary<string, object> envDict)
        {
            foreach (var pair in envDict)
            {
                GetType().GetProperty(pair.Key)?.SetValue(this, pair.Value);
            }
        }

        public override void Deploy(Device device)
        {
            if (CallIn)
            {
                DeployCallIn(device);
            }
            else
            {
                DeployCallOut(device);
            }
        }

        private void DeployCallIn(Device device)
        {
            if (!device.ReceiveCall(Phone))
            {
                return;
            }
            Thread.Sleep(1000);
            if (Accepted)
            {
                device.AcceptCall(Phone);
                Thread.Sleep(1000);
            }
            device.CancelCall(Phone);
        }

        private void DeployCallOut(Device device)
        {
            device.Call(Phone);
            Thread.Sleep(2000);
            device.CancelCall(Phone);
        }
    }

    public class DummyFilesEnv : StaticAppEnv
    {
        public string DummyFilesDir { get; set; }

        public string EnvType { get; set; } = "dummy_files";

        public DummyFilesEnv(string dummyFilesDir = null)
        {
            if (dummyFilesDir == null)
            {
                dummyFilesDir = "path_to_default_dummy_files";
            }
            DummyFilesDir = dummyFilesDir;
        }

        public override void Deploy(Device device)
        {
            device.PushFile(DummyFilesDir);
        }
    }

    public class SMSLogEnv : StaticAppEnv
    {
        public string Phone { get; set; } = "1234567890";
        public bool SmsIn { get; set; } = true;
        public string Content { get; set; } = "Hello world";
        public string EnvType { get; set; } = "smslog";

        public SMSLogEnv() { }

        public SMSLogEnv(Dictionary<string, object> envDict)
        {
            foreach (var pair in envDict)
            {
                GetType().GetProperty(pair.Key)?.SetValue(this, pair.Value);
            }
        }

        public override void Deploy(Device device)
        {
            if (SmsIn)
            {
                device.ReceiveSms(Phone, Content);
            }
            else
            {
                device.SendSms(Phone, Content);
            }
        }
    }

    public class GPSAppEnv : DynamicAppEnv
    {
        public float CenterX { get; set; } = 50;
        public float CenterY { get; set; } = 50;
        public float DeltaX { get; set; } = 1;
        public float DeltaY { get; set; } = 1;
        public string EnvType { get; set; } = "gps";

        public GPSAppEnv() { }

        public GPSAppEnv(Dictionary<string, object> envDict)
        {
            foreach (var pair in envDict)
            {
                GetType().GetProperty(pair.Key)?.SetValue(this, pair.Value);
            }
        }

        public override void Deploy(Device device)
        {
            device.SetContinuousGps(CenterX, CenterY, DeltaX, DeltaY);
        }
    }

    public class AppEnvManager
    {
        private readonly Device device;
        private readonly App app;
        private readonly string policy;
        private List<AppEnv> envs;
        private bool enabled;
        private AppEnvFactory envFactory;

        public AppEnvManager(Device device, App app, string envPolicy)
        {
            this.device = device;
            this.app = app;
            this.policy = envPolicy ?? "none";
            this.envs = new List<AppEnv>();
            this.enabled = true;

            switch (this.policy)
            {
                case "none":
                    envFactory = null;
                    break;
                case "dummy":
                    envFactory = new DummyEnvFactory();
                    break;
                case "static":
                    envFactory = new StaticEnvFactory(app);
                    break;
                default:
                    throw new ArgumentException("Unknown environment policy");
            }
        }

        public void AddEnv(AppEnv env)
        {
            envs.Add(env);
        }

        public void Deploy()
        {
            if (!enabled) return;

            if (envFactory != null)
            {
                envs = GenerateFromFactory(envFactory);
            }

            foreach (var env in envs)
            {
                if (!enabled) break;
                device.AddEnv(env);
            }

            if (device.OutputDir != null)
            {
                var outputFile = Path.Combine(device.OutputDir, "droidbot_env.json");
                File.WriteAllText(outputFile, JsonConvert.SerializeObject(envs));
            }
        }

        private List<AppEnv> GenerateFromFactory(AppEnvFactory appEnvFactory)
        {
            return appEnvFactory.ProduceEnvs();
        }

        public void Stop()
        {
            enabled = false;
        }
    }

    public abstract class AppEnvFactory
    {
        public abstract List<AppEnv> ProduceEnvs();
    }

    public class DummyEnvFactory : AppEnvFactory
    {
        public override List<AppEnv> ProduceEnvs()
        {
            return new List<AppEnv>
            {
                new ContactAppEnv(),
                new SettingsAppEnv(),
                new CallLogEnv(),
                new SMSLogEnv(),
                new GPSAppEnv(),
                new DummyFilesEnv()
            };
        }
    }

    public class StaticEnvFactory : AppEnvFactory
    {
        private readonly App app;

        public StaticEnvFactory(App app)
        {
            this.app = app;
        }

        public override List<AppEnv> ProduceEnvs()
        {
            var envs = new List<AppEnv>();

            if (app.Permissions.Contains("android.permission.READ_CONTACTS"))
            {
                envs.Add(new ContactAppEnv());
            }

            if (app.Permissions.Contains("android.permission.READ_CALL_LOG"))
            {
                envs.Add(new CallLogEnv());
                envs.Add(new CallLogEnv { CallIn = false });
                envs.Add(new CallLogEnv { Accepted = false });
            }

            if (app.Permissions.Contains("android.permission.ACCESS_FINE_LOCATION"))
            {
                envs.Add(new GPSAppEnv());
            }

            if (app.Permissions.Contains("android.permission.READ_SMS"))
            {
                envs.Add(new SMSLogEnv());
                envs.Add(new SMSLogEnv { SmsIn = false });
            }

            if (app.Permissions.Contains("android.permission.READ_EXTERNAL_STORAGE") ||
                app.Permissions.Contains("android.permission.WRITE_EXTERNAL_STORAGE") ||
                app.Permissions.Contains("android.permission.MOUNT_UNMOUNT_FILESYSTEMS"))
            {
                envs.Add(new DummyFilesEnv());
            }

            return envs;
        }
    }
}

