using Image = System.Drawing.Image;
using nTestar.Desktop.Winforms.mvp;

namespace nTestar.Desktop.Winforms
{
    public partial class MainForm : Form, IMainView
    {
        public event EventHandler SelectSutRequested;
        public event EventHandler EditProtocolRequested;
        public event EventHandler SpyModeRequested;
        public event EventHandler GenerateModeRequested;
        public event EventHandler ReplayModeRequested;
        public event EventHandler ViewReportRequested;
        public event EventHandler ModelModeRequested;

        public MainForm()
        {
            Text = "TESTAR 2.7.5 (15-Jul-2025)";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            ClientSize = new Size(1260, 900);
            MinimumSize = new Size(1180, 820);
            BackColor = Color.FromArgb(210, 215, 222);
            Font = new Font("Segoe UI", 10.5f, FontStyle.Regular);
            AutoScaleMode = AutoScaleMode.Dpi;

            InitializeComponent();
        }

        public string SutConnector
        {
            get => _sutConnectorTextBox.Text;
            set => _sutConnectorTextBox.Text = value;
        }

        public int NumberOfSequences
        {
            get => (int)numberOfSequencesUpDown.Value;
            set => numberOfSequencesUpDown.Value = value;
        }
        public int SequenceActions
        {
            get => (int)sequenceActionsUpDown.Value;
            set => sequenceActionsUpDown.Value = value;
        }

        //public bool VisualizeActionsOnGui
        //{
        //    //get => visualizeActionsCheckBox.Checked;
        //    //set => visualizeActionsCheckBox.Checked = value;


        //}

        public bool AlwaysCompileProtocol
        {
            get => alwaysCompileProtocolCheckBox.Checked;
            set => alwaysCompileProtocolCheckBox.Checked = value;
        }

        //public string SelectedProtocol
        //{
        //    get => always.Text;
        //    set => _protocolComboBox.Text = value;
        //}

        //public string ApplicationName
        //{
        //    get => _applicationNameTextBox.Text;
        //    set => _applicationNameTextBox.Text = value;
        //}

        //public string ApplicationVersion
        //{
        //    get => _applicationVersionTextBox.Text;
        //    set => _applicationVersionTextBox.Text = value;
        //}

        //public string OverrideDisplayScale
        //{
        //    get => _overrideDisplayScaleTextBox.Text;
        //    set => _overrideDisplayScaleTextBox.Text = value;
        //}

        public void SetProtocols(IEnumerable<string> protocols)
        {
            //_protocolComboBox.Items.Clear();
            //foreach (var protocol in protocols)
            //    _protocolComboBox.Items.Add(protocol);
        }
        public void ShowInfo(string message, string title)
        {
            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}
