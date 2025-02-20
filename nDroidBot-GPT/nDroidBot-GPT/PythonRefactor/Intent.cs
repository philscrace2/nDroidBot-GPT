using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nDroidBot_GPT.PythonRefactor
{
    public class Intent
    {
        public string EventType { get; set; } = "intent";
        public string Prefix { get; set; } = "start";
        public string Action { get; set; }
        public string DataUri { get; set; }
        public string MimeType { get; set; }
        public string Category { get; set; }
        public string Component { get; set; }
        public string Flag { get; set; }
        public Dictionary<string, string> ExtraString { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, bool> ExtraBoolean { get; set; } = new Dictionary<string, bool>();
        public Dictionary<string, int> ExtraInt { get; set; } = new Dictionary<string, int>();
        public List<string> Flags { get; set; } = new List<string>();
        public string Suffix { get; set; }
        public string Cmd { get; set; }

        public string GetCmd()
        {
            if (Cmd != null)
                return Cmd;

            string cmd = "am ";
            if (!string.IsNullOrEmpty(Prefix))
                cmd += Prefix;
            if (Action != null)
                cmd += " -a " + Action;
            if (DataUri != null)
                cmd += " -d " + DataUri;
            if (MimeType != null)
                cmd += " -t " + MimeType;
            if (Category != null)
                cmd += " -c " + Category;
            if (Component != null)
                cmd += " -n " + Component;
            if (Flag != null)
                cmd += " -f " + Flag;

            foreach (var extra in ExtraString)
                cmd += $" -e '{extra.Key}' '{extra.Value}'";

            foreach (var extra in ExtraInt)
                cmd += $" -ei '{extra.Key}' {extra.Value}";

            foreach (var flag in Flags)
                cmd += " " + flag;

            if (!string.IsNullOrEmpty(Suffix))
                cmd += " " + Suffix;

            Cmd = cmd;
            return Cmd;
        }

        public override string ToString() => GetCmd();
    }

}
