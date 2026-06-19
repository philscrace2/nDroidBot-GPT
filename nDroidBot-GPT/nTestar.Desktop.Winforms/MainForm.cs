using Image = System.Drawing.Image;
using nTestar.Desktop.Winforms.mvp;

namespace nTestar.Desktop.Winforms
{
    public partial class MainForm : Form, IMainView
    {
        private bool _suppressProtocolSelectionEvent;

        public event EventHandler SelectSutRequested;
        public event EventHandler EditProtocolRequested;
        public event EventHandler SpyModeRequested;
        public event EventHandler GenerateModeRequested;
        public event EventHandler ReplayModeRequested;
        public event EventHandler ViewReportRequested;
        public event EventHandler ModelModeRequested;
        public event EventHandler ProtocolSelectionChanged;

        public MainForm()
        {
            InitializeComponent();

            Text = "TESTAR 2.7.5 (15-Jul-2025)";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            ClientSize = new Size(1260, 900);
            MinimumSize = new Size(1180, 820);
            BackColor = Color.FromArgb(210, 215, 222);
            Font = new Font("Segoe UI", 10.5f, FontStyle.Regular);
            AutoScaleMode = AutoScaleMode.Dpi;

            WireUiEvents();
        }

        public string SutConnector
        {
            get => _sutConnectorTextBox.Text;
            set => _sutConnectorTextBox.Text = value;
        }

        public string SutConnectorType
        {
            get => _sutConnectorComboBox.Text;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !_sutConnectorComboBox.Items.Contains(value))
                {
                    _sutConnectorComboBox.Items.Add(value);
                }

                _sutConnectorComboBox.Text = value ?? string.Empty;
            }
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

        public string SelectedProtocol
        {
            get => comboBox1.Text;
            set => comboBox1.Text = value;
        }

        public string ApplicationName
        {
            get => applicationNameTextBox.Text;
            set => applicationNameTextBox.Text = value;
        }

        public string ApplicationVersion
        {
            get => applicationVersionTextBox.Text;
            set => applicationVersionTextBox.Text = value;
        }

        //public string OverrideDisplayScale
        //{
        //    get => _overrideDisplayScaleTextBox.Text;
        //    set => _overrideDisplayScaleTextBox.Text = value;
        //}

        public void SetProtocols(IEnumerable<string> protocols)
        {
            _suppressProtocolSelectionEvent = true;
            comboBox1.Items.Clear();
            foreach (var protocol in protocols)
            {
                comboBox1.Items.Add(protocol);
            }
            _suppressProtocolSelectionEvent = false;
        }

        public void SetSutConnectors(IEnumerable<string> connectors)
        {
            _sutConnectorComboBox.Items.Clear();
            foreach (var connector in connectors)
            {
                _sutConnectorComboBox.Items.Add(connector);
            }
        }
        public void ShowInfo(string message, string title)
        {
            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void WireUiEvents()
        {
            selectSUTButton.Click += (_, _) => SelectSutRequested?.Invoke(this, EventArgs.Empty);
            button6.Click += (_, _) => EditProtocolRequested?.Invoke(this, EventArgs.Empty);
            spyModeButton.Click += (_, _) => SpyModeRequested?.Invoke(this, EventArgs.Empty);
            startInGenerateMode.Click += (_, _) => GenerateModeRequested?.Invoke(this, EventArgs.Empty);
            replayModeButton.Click += (_, _) => ReplayModeRequested?.Invoke(this, EventArgs.Empty);
            viewModeButton.Click += (_, _) => ViewReportRequested?.Invoke(this, EventArgs.Empty);
            stateModelAnalysisModeButton.Click += (_, _) => ModelModeRequested?.Invoke(this, EventArgs.Empty);
            comboBox1.SelectedIndexChanged += (_, _) =>
            {
                if (_suppressProtocolSelectionEvent)
                {
                    return;
                }

                ProtocolSelectionChanged?.Invoke(this, EventArgs.Empty);
            };
        }
    }
}
