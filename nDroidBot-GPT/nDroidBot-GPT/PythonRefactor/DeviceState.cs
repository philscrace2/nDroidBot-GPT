using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace nDroidBot_GPT.PythonRefactor
{
    public class DeviceState
    {
        private Device device;
        private string foregroundActivity;
        private List<string> activityStack;
        private List<View> views;

        public DeviceState(Device device, List<View> views, string foregroundActivity, List<string> activityStack)
        {
            this.device = device;
            this.foregroundActivity = foregroundActivity;
            this.activityStack = activityStack;
            this.views = views;
        }

        public string GetStateString()
        {
            var stateStr = string.Join(",", views.Select(view => view.GetViewSignature()));
            return ComputeMD5Hash(stateStr);
        }

        public static string ComputeMD5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        public List<string> GetPossibleEvents()
        {
            List<string> possibleEvents = new List<string>();

            foreach (var view in views)
            {
                if (view.IsClickable)
                    possibleEvents.Add($"Click on {view.GetViewSignature()}");

                if (view.IsScrollable)
                    possibleEvents.Add($"Scroll {view.GetViewSignature()}");

                if (view.IsEditable)
                    possibleEvents.Add($"Edit {view.GetViewSignature()}");
            }

            return possibleEvents;
        }
    }

    public class View
    {
        public string ResourceId { get; set; }
        public string Text { get; set; }
        public string Class { get; set; }
        public bool IsClickable { get; set; }
        public bool IsScrollable { get; set; }
        public bool IsEditable { get; set; }

        public string GetViewSignature()
        {
            return $"{Class}-{ResourceId}-{Text}";
        }
    }

}
