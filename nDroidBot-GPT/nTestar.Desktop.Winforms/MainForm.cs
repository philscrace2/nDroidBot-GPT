using Image = System.Drawing.Image;
using nTestar.Desktop.Winforms.mvp;

namespace nTestar.Desktop.Winforms
{
    public partial class MainForm : Form, IMainView
    {
        private TextBox _sutConnectorTextBox;
        private NumericUpDown _numberOfSequencesNumeric;
        private NumericUpDown _sequenceActionsNumeric;
        private CheckBox _visualizeActionsCheckBox;
        private CheckBox _alwaysCompileProtocolCheckBox;
        private ComboBox _protocolComboBox;
        private TextBox _applicationNameTextBox;
        private TextBox _applicationVersionTextBox;
        private TextBox _overrideDisplayScaleTextBox;
        private TabControl _tabControl;

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

            BuildLayout();
        }

        private void BuildLayout()
        {
            SuspendLayout();
            Controls.Clear();

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Color.FromArgb(210, 215, 222),
                Padding = new Padding(22, 18, 22, 14)
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 255));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            root.Controls.Add(BuildModeBar(), 0, 0);
            root.Controls.Add(BuildTabStrip(), 0, 1);
            root.Controls.Add(BuildSettingsPanel(), 0, 2);

            Controls.Add(root);
            ResumeLayout(true);
        }

        private Control BuildModeBar()
        {
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = false,
                BackColor = Color.White,
                Padding = new Padding(24, 10, 24, 10),
                Margin = new Padding(0)
            };

            panel.Controls.Add(CreateModeButton("Spy", CreateSpyImage(), "Start in SPY Mode:This mode allows you to inspect the GUI of the System Under Test.", SpyModeRequested));
            panel.Controls.Add(CreateModeButton("Generate", CreateCycleImage(), "Start in Generate Mode.", GenerateModeRequested));
            panel.Controls.Add(CreateModeButton("Replay", CreateReplayImage(), "Replay an existing sequence.", ReplayModeRequested));
            panel.Controls.Add(CreateModeButton("Report", CreateReportImage(), "View or inspect generated reports.", ViewReportRequested));
            panel.Controls.Add(CreateModeButton("Model", CreateModelImage(), "Inspect the generated state model.", ModelModeRequested));

            return panel;
        }

        private Control CreateModeButton(string caption, Image image, string tooltip, EventHandler eventHandler)
        {
            var button = new ModeButtonControl(caption, tooltip, image)
            {
                Width = 225,
                Height = 225,
                Margin = new Padding(6)
            };
            button.Click += (s, e) => eventHandler?.Invoke(this, EventArgs.Empty);
            return button;
        }

        private Control BuildTabStrip()
        {
            var host = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Margin = new Padding(0, 10, 0, 0),
                BackColor = Color.FromArgb(210, 215, 222)
            };

            host.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 76));
            host.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24));
            host.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            host.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            host.Controls.Add(CreateTabButton("Time Settings"), 0, 0);
            host.Controls.Add(CreateTabButton("Advanced Options"), 1, 0);
            host.Controls.Add(CreateTabButton("About"), 0, 1);
            host.Controls.Add(CreateTabButton("Oracles"), 1, 1);

            return host;
        }

        private static Button CreateTabButton(string text)
        {
            return new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                Margin = new Padding(0),
                FlatStyle = FlatStyle.Standard,
                Font = new Font("Segoe UI", 11f, FontStyle.Regular)
            };
        }

        private static Label CreateGridLabel(string text)
        {
            return new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopLeft,
                Padding = new Padding(0, 8, 0, 0),
                Font = new Font("Segoe UI", 11f)
            };
        }

        private static NumericUpDown CreateNumeric(int min, int max)
        {
            return new NumericUpDown
            {
                Dock = DockStyle.Top,
                Height = 34,
                Minimum = min,
                Maximum = max,
                Font = new Font("Segoe UI", 11f),
                TextAlign = HorizontalAlignment.Right,
                Margin = new Padding(0, 4, 36, 0)
            };
        }



        public string SutConnector
        {
            get => _sutConnectorTextBox.Text;
            set => _sutConnectorTextBox.Text = value;
        }

        public int NumberOfSequences
        {
            get => (int)_numberOfSequencesNumeric.Value;
            set => _numberOfSequencesNumeric.Value = value;
        }
        public int SequenceActions
        {
            get => (int)_sequenceActionsNumeric.Value;
            set => _sequenceActionsNumeric.Value = value;
        }

        public bool VisualizeActionsOnGui
        {
            get => _visualizeActionsCheckBox.Checked;
            set => _visualizeActionsCheckBox.Checked = value;
        }

        public bool AlwaysCompileProtocol
        {
            get => _alwaysCompileProtocolCheckBox.Checked;
            set => _alwaysCompileProtocolCheckBox.Checked = value;
        }

        public string SelectedProtocol
        {
            get => _protocolComboBox.Text;
            set => _protocolComboBox.Text = value;
        }

        public string ApplicationName
        {
            get => _applicationNameTextBox.Text;
            set => _applicationNameTextBox.Text = value;
        }

        public string ApplicationVersion
        {
            get => _applicationVersionTextBox.Text;
            set => _applicationVersionTextBox.Text = value;
        }

        public string OverrideDisplayScale
        {
            get => _overrideDisplayScaleTextBox.Text;
            set => _overrideDisplayScaleTextBox.Text = value;
        }

        private Control BuildSettingsPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(210, 215, 222),
                Padding = new Padding(5, 15, 30, 20)
            };

            var sutLabel = new Label
            {
                Text = "SUT connector:",
                AutoSize = true,
                Location = new Point(0, 18),
                Font = new Font("Segoe UI", 11f)
            };

            var selectSutButton = new Button
            {
                Text = "Select SUT",
                Size = new Size(170, 42),
                Location = new Point(panel.Width - 210, 6),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Font = new Font("Segoe UI", 11f)
            };
            selectSutButton.Click += (s, e) => SelectSutRequested?.Invoke(this, EventArgs.Empty);

            _sutConnectorTextBox = new TextBox
            {
                Multiline = true,
                Location = new Point(5, 65),
                Size = new Size(1160, 205),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 11f),
                ScrollBars = ScrollBars.Vertical
            };

            var leftColumnX = 0;
            var numericX = 300;
            var midColumnX = 535;
            var inputX = 920;
            var y1 = 300;
            var row = 76;

            var sequencesLabel = CreateLabel("Number of Sequences:", leftColumnX, y1);
            _numberOfSequencesNumeric = CreateNumeric(numericX, y1 - 6, 1, 100000);

            var actionsLabel = CreateLabel("Sequence actions:", leftColumnX, y1 + row);
            _sequenceActionsNumeric = CreateNumeric(numericX, y1 + row - 6, 1, 100000);

            _visualizeActionsCheckBox = new CheckBox
            {
                Text = "Visualize actions on GUI",
                Location = new Point(leftColumnX, y1 + row * 2),
                AutoSize = true,
                Font = new Font("Segoe UI", 11f)
            };

            var protocolLabel = CreateLabel("Protocol:", midColumnX, y1);
            _protocolComboBox = new ComboBox
            {
                Location = new Point(midColumnX + 130, y1 - 6),
                Size = new Size(500, 36),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                DropDownStyle = ComboBoxStyle.DropDown,
                Font = new Font("Segoe UI", 11f)
            };

            _alwaysCompileProtocolCheckBox = new CheckBox
            {
                Text = "Always compile protocol",
                Location = new Point(midColumnX, y1 + row),
                AutoSize = true,
                Font = new Font("Segoe UI", 11f)
            };

            var editProtocolButton = new Button
            {
                Text = "Edit Protocol",
                Location = new Point(inputX + 60, y1 + row - 6),
                Size = new Size(190, 42),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Font = new Font("Segoe UI", 11f)
            };
            editProtocolButton.Click += (s, e) => EditProtocolRequested?.Invoke(this, EventArgs.Empty);

            var appNameLabel = CreateLabel("Application name", midColumnX + 80, y1 + row * 2);
            _applicationNameTextBox = CreateTextBox(inputX, y1 + row * 2 - 8);

            var appVersionLabel = CreateLabel("Application version", midColumnX + 80, y1 + row * 3);
            _applicationVersionTextBox = CreateTextBox(inputX, y1 + row * 3 - 8);

            var scaleLabel = CreateLabel("Override display scale", midColumnX + 80, y1 + row * 4);
            _overrideDisplayScaleTextBox = CreateTextBox(inputX, y1 + row * 4 - 8);

            panel.Controls.Add(sutLabel);
            panel.Controls.Add(selectSutButton);
            panel.Controls.Add(_sutConnectorTextBox);
            panel.Controls.Add(sequencesLabel);
            panel.Controls.Add(_numberOfSequencesNumeric);
            panel.Controls.Add(actionsLabel);
            panel.Controls.Add(_sequenceActionsNumeric);
            panel.Controls.Add(_visualizeActionsCheckBox);
            panel.Controls.Add(protocolLabel);
            panel.Controls.Add(_protocolComboBox);
            panel.Controls.Add(_alwaysCompileProtocolCheckBox);
            panel.Controls.Add(editProtocolButton);
            panel.Controls.Add(appNameLabel);
            panel.Controls.Add(_applicationNameTextBox);
            panel.Controls.Add(appVersionLabel);
            panel.Controls.Add(_applicationVersionTextBox);
            panel.Controls.Add(scaleLabel);
            panel.Controls.Add(_overrideDisplayScaleTextBox);

            return panel;
        }

        private static Label CreateLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 11f)
            };
        }

        private static NumericUpDown CreateNumeric(int x, int y, int min, int max)
        {
            return new NumericUpDown
            {
                Location = new Point(x, y),
                Size = new Size(130, 36),
                Minimum = min,
                Maximum = max,
                Font = new Font("Segoe UI", 11f),
                TextAlign = HorizontalAlignment.Right
            };
        }

        private static TextBox CreateTextBox(int x, int y)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(245, 36),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Font = new Font("Segoe UI", 11f)
            };
        }

        private static Image CreateSpyImage()
        {
            var bmp = NewImage();
            using (var g = Graphics.FromImage(bmp))
            using (var black = new SolidBrush(Color.Black))
            using (var pen = new Pen(Color.Black, 6))
            {
                g.FillPolygon(black, new[]
                {
                    new Point(48, 92), new Point(176, 92), new Point(210, 120), new Point(14, 120)
                });
                g.FillPolygon(black, new[]
                {
                    new Point(75, 40), new Point(150, 40), new Point(165, 92), new Point(60, 92)
                });
                g.DrawEllipse(pen, 63, 130, 48, 48);
                g.DrawEllipse(pen, 122, 130, 48, 48);
                g.DrawLine(pen, 111, 154, 122, 154);
                g.DrawArc(pen, 85, 175, 62, 34, 0, 180);
                g.DrawLine(pen, 116, 168, 116, 185);
            }
            return bmp;
        }

        private static Image CreateCycleImage()
        {
            var bmp = NewImage();
            using (var g = Graphics.FromImage(bmp))
            using (var pen = new Pen(Color.DodgerBlue, 22))
            using (var greenPen = new Pen(Color.LimeGreen, 22))
            using (var redPen = new Pen(Color.OrangeRed, 22))
            using (var yellowPen = new Pen(Color.Gold, 22))
            using (var brush = new SolidBrush(Color.DodgerBlue))
            {
                g.DrawArc(greenPen, 38, 35, 150, 150, 160, 95);
                g.DrawArc(pen, 38, 35, 150, 150, 250, 80);
                g.DrawArc(redPen, 38, 35, 150, 150, 330, 70);
                g.DrawArc(yellowPen, 38, 35, 150, 150, 40, 85);
                g.FillEllipse(Brushes.LimeGreen, 36, 105, 18, 18);
                g.FillEllipse(Brushes.DodgerBlue, 95, 20, 18, 18);
                g.FillEllipse(Brushes.OrangeRed, 168, 55, 18, 18);
                g.FillEllipse(Brushes.Gold, 172, 113, 18, 18);
            }
            return bmp;
        }

        private static Image CreateReplayImage()
        {
            var bmp = NewImage();
            using (var g = Graphics.FromImage(bmp))
            using (var pen = new Pen(Color.FromArgb(0, 83, 165), 30))
            using (var brush = new SolidBrush(Color.FromArgb(0, 83, 165)))
            {
                g.DrawArc(pen, 42, 35, 135, 135, 45, 285);
                g.FillPolygon(brush, new[] { new Point(148, 20), new Point(210, 70), new Point(148, 120) });
                g.FillPolygon(brush, new[] { new Point(95, 86), new Point(155, 122), new Point(95, 158) });
            }
            return bmp;
        }

        private static Image CreateReportImage()
        {
            var bmp = NewImage();
            using (var g = Graphics.FromImage(bmp))
            using (var pen = new Pen(Color.Black, 3))
            using (var eye = new SolidBrush(Color.SeaGreen))
            {
                g.DrawRectangle(pen, 58, 62, 88, 145);
                g.DrawLine(pen, 146, 62, 166, 82);
                g.DrawLine(pen, 166, 82, 166, 207);
                g.DrawRectangle(pen, 72, 92, 78, 30);
                for (var y = 146; y < 190; y += 10) g.DrawLine(pen, 72, y, 150, y);
                g.DrawArc(pen, 138, 35, 78, 58, 200, 140);
                g.DrawArc(pen, 138, 35, 78, 58, 20, 140);
                g.FillEllipse(eye, 169, 53, 28, 28);
                g.FillEllipse(Brushes.Black, 178, 61, 11, 11);
            }
            return bmp;
        }

        private static Image CreateModelImage()
        {
            var bmp = NewImage();
            using (var g = Graphics.FromImage(bmp))
            using (var teal = new SolidBrush(Color.FromArgb(34, 151, 156)))
            using (var pen = new Pen(Color.FromArgb(34, 151, 156), 4))
            {
                g.FillEllipse(teal, 25, 70, 118, 118);
                g.FillEllipse(teal, 145, 130, 70, 70);
                g.DrawArc(pen, 115, 96, 75, 85, 275, 110);
                g.DrawArc(pen, 95, 150, 80, 75, 85, 115);
                g.DrawArc(Pens.Black, 145, 25, 78, 58, 200, 140);
                g.DrawArc(Pens.Black, 145, 25, 78, 58, 20, 140);
                g.FillEllipse(Brushes.SeaGreen, 176, 43, 28, 28);
                g.FillEllipse(Brushes.Black, 184, 51, 11, 11);
            }
            return bmp;
        }
        private static Bitmap NewImage()
        {
            var bmp = new Bitmap(224, 224);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            }
            return bmp;
        }

        public void SetProtocols(IEnumerable<string> protocols)
        {
            _protocolComboBox.Items.Clear();
            foreach (var protocol in protocols)
                _protocolComboBox.Items.Add(protocol);
        }
        public void ShowInfo(string message, string title)
        {
            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}
