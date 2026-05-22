using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nTestar.Desktop.Winforms.mvp
{
    public interface IMainView
    {
        event EventHandler SelectSutRequested;
        event EventHandler EditProtocolRequested;
        event EventHandler SpyModeRequested;
        event EventHandler GenerateModeRequested;
        event EventHandler ReplayModeRequested;
        event EventHandler ViewReportRequested;
        event EventHandler ModelModeRequested;

        public string SutConnector { get; set; }
        public int NumberOfSequences { get; set; }
        public int SequenceActions { get; set; }
        //public bool VisualizeActionsOnGui { get; set; }
        public bool AlwaysCompileProtocol { get; set; }
        //public string SelectedProtocol { get; set; }
        //public string ApplicationName { get; set; }
        //public string ApplicationVersion { get; set; }
        //public string OverrideDisplayScale { get; set; }

        public void SetProtocols(IEnumerable<string> protocols);
        public void ShowInfo(string message, string title);
    }
}
