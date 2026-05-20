using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using nTestar.Desktop.Winforms.mvp;

namespace nTestar.Desktop.Winforms
{
    public partial class MainFormWinForms : Form, IMainView
    {
        public MainFormWinForms()
        {
            InitializeComponent();
        }

        public string SutConnector { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int NumberOfSequences { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int SequenceActions { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool VisualizeActionsOnGui { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AlwaysCompileProtocol { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string SelectedProtocol { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ApplicationVersion { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string OverrideDisplayScale { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event EventHandler SelectSutRequested;
        public event EventHandler EditProtocolRequested;
        public event EventHandler SpyModeRequested;
        public event EventHandler GenerateModeRequested;
        public event EventHandler ReplayModeRequested;
        public event EventHandler ViewReportRequested;
        public event EventHandler ModelModeRequested;

        public void SetProtocols(IEnumerable<string> protocols)
        {

        }

        public void ShowInfo(string message, string title)
        {

        }
    }
}
