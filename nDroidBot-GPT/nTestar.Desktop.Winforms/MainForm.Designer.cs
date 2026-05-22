namespace nTestar.Desktop.Winforms
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            spyModeButton = new Button();
            startInGenerateMode = new Button();
            replayModeButton = new Button();
            viewModeButton = new Button();
            stateModelAnalysisModeButton = new Button();
            tabControlGeneralSettings = new TabControl();
            tabPage1 = new TabPage();
            tableLayoutPanel3 = new TableLayoutPanel();
            tableLayoutPanel4 = new TableLayoutPanel();
            label1 = new Label();
            selectSUTButton = new Button();
            _sutConnectorComboBox = new ComboBox();
            _sutConnectorTextBox = new RichTextBox();
            tableLayoutPanel5 = new TableLayoutPanel();
            tableLayoutPanel6 = new TableLayoutPanel();
            numberOfSequencesLabel = new Label();
            label2 = new Label();
            numberOfSequencesUpDown = new NumericUpDown();
            sequenceActionsUpDown = new NumericUpDown();
            tableLayoutPanel7 = new TableLayoutPanel();
            protocolLabel = new Label();
            comboBox1 = new ComboBox();
            alwaysCompileProtocolCheckBox = new CheckBox();
            button6 = new Button();
            tableLayoutPanel8 = new TableLayoutPanel();
            applicationNameLabel = new Label();
            applicationVersionLabel = new Label();
            applicationNameTextBox = new TextBox();
            applicationVersionTextBox = new TextBox();
            tabPage2 = new TabPage();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tabControlGeneralSettings.SuspendLayout();
            tabPage1.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            tableLayoutPanel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numberOfSequencesUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)sequenceActionsUpDown).BeginInit();
            tableLayoutPanel7.SuspendLayout();
            tableLayoutPanel8.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 0);
            tableLayoutPanel1.Controls.Add(tabControlGeneralSettings, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 37.2093F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 62.7907F));
            tableLayoutPanel1.Size = new Size(3211, 1290);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 5;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel2.Controls.Add(spyModeButton, 0, 0);
            tableLayoutPanel2.Controls.Add(startInGenerateMode, 1, 0);
            tableLayoutPanel2.Controls.Add(replayModeButton, 2, 0);
            tableLayoutPanel2.Controls.Add(viewModeButton, 3, 0);
            tableLayoutPanel2.Controls.Add(stateModelAnalysisModeButton, 4, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(3205, 474);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // spyModeButton
            // 
            spyModeButton.Dock = DockStyle.Fill;
            spyModeButton.Image = Resource1.button_spy;
            spyModeButton.Location = new Point(3, 3);
            spyModeButton.Name = "spyModeButton";
            spyModeButton.Size = new Size(635, 468);
            spyModeButton.TabIndex = 0;
            spyModeButton.UseVisualStyleBackColor = true;
            // 
            // startInGenerateMode
            // 
            startInGenerateMode.Dock = DockStyle.Fill;
            startInGenerateMode.Image = Resource1.button_generate;
            startInGenerateMode.Location = new Point(644, 3);
            startInGenerateMode.Name = "startInGenerateMode";
            startInGenerateMode.Size = new Size(635, 468);
            startInGenerateMode.TabIndex = 1;
            startInGenerateMode.UseVisualStyleBackColor = true;
            // 
            // replayModeButton
            // 
            replayModeButton.Dock = DockStyle.Fill;
            replayModeButton.Image = Resource1.button_replay;
            replayModeButton.Location = new Point(1285, 3);
            replayModeButton.Name = "replayModeButton";
            replayModeButton.Size = new Size(635, 468);
            replayModeButton.TabIndex = 2;
            replayModeButton.UseVisualStyleBackColor = true;
            // 
            // viewModeButton
            // 
            viewModeButton.Dock = DockStyle.Fill;
            viewModeButton.Image = Resource1.view_report;
            viewModeButton.Location = new Point(1926, 3);
            viewModeButton.Name = "viewModeButton";
            viewModeButton.Size = new Size(635, 468);
            viewModeButton.TabIndex = 3;
            viewModeButton.UseVisualStyleBackColor = true;
            // 
            // stateModelAnalysisModeButton
            // 
            stateModelAnalysisModeButton.Dock = DockStyle.Fill;
            stateModelAnalysisModeButton.Image = Resource1.view_model;
            stateModelAnalysisModeButton.Location = new Point(2567, 3);
            stateModelAnalysisModeButton.Name = "stateModelAnalysisModeButton";
            stateModelAnalysisModeButton.Size = new Size(635, 468);
            stateModelAnalysisModeButton.TabIndex = 4;
            stateModelAnalysisModeButton.UseVisualStyleBackColor = true;
            // 
            // tabControlGeneralSettings
            // 
            tabControlGeneralSettings.Controls.Add(tabPage1);
            tabControlGeneralSettings.Controls.Add(tabPage2);
            tabControlGeneralSettings.Dock = DockStyle.Fill;
            tabControlGeneralSettings.Location = new Point(3, 483);
            tabControlGeneralSettings.Name = "tabControlGeneralSettings";
            tabControlGeneralSettings.SelectedIndex = 0;
            tabControlGeneralSettings.Size = new Size(3205, 804);
            tabControlGeneralSettings.TabIndex = 1;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(tableLayoutPanel3);
            tabPage1.Location = new Point(10, 58);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(3185, 736);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "General Settings";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 1;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Controls.Add(tableLayoutPanel4, 0, 0);
            tableLayoutPanel3.Controls.Add(_sutConnectorTextBox, 0, 1);
            tableLayoutPanel3.Controls.Add(tableLayoutPanel5, 0, 2);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(3, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 3;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 18.186451F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 29.8630142F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 52.0547943F));
            tableLayoutPanel3.Size = new Size(3179, 730);
            tableLayoutPanel3.TabIndex = 0;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 3;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10.057003F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 89.94299F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 852F));
            tableLayoutPanel4.Controls.Add(label1, 0, 0);
            tableLayoutPanel4.Controls.Add(selectSUTButton, 2, 0);
            tableLayoutPanel4.Controls.Add(_sutConnectorComboBox, 1, 0);
            tableLayoutPanel4.Location = new Point(3, 3);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.Size = new Size(3173, 113);
            tableLayoutPanel4.TabIndex = 1;
            // 
            // label1
            // 
            label1.Dock = DockStyle.Fill;
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(227, 113);
            label1.TabIndex = 0;
            label1.Text = "SUT connector:";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // selectSUTButton
            // 
            selectSUTButton.Dock = DockStyle.Fill;
            selectSUTButton.Location = new Point(2323, 3);
            selectSUTButton.Name = "selectSUTButton";
            selectSUTButton.Size = new Size(847, 107);
            selectSUTButton.TabIndex = 1;
            selectSUTButton.Text = "Select SUT";
            selectSUTButton.UseVisualStyleBackColor = true;
            // 
            // _sutConnectorComboBox
            // 
            _sutConnectorComboBox.Anchor = AnchorStyles.None;
            _sutConnectorComboBox.FormattingEnabled = true;
            _sutConnectorComboBox.Location = new Point(236, 32);
            _sutConnectorComboBox.Name = "_sutConnectorComboBox";
            _sutConnectorComboBox.Size = new Size(2081, 49);
            _sutConnectorComboBox.TabIndex = 2;
            // 
            // _sutConnectorTextBox
            // 
            _sutConnectorTextBox.Dock = DockStyle.Fill;
            _sutConnectorTextBox.Location = new Point(3, 135);
            _sutConnectorTextBox.Name = "_sutConnectorTextBox";
            _sutConnectorTextBox.Size = new Size(3173, 211);
            _sutConnectorTextBox.TabIndex = 2;
            _sutConnectorTextBox.Text = "";
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.ColumnCount = 2;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 27.734005F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 72.26599F));
            tableLayoutPanel5.Controls.Add(tableLayoutPanel6, 0, 0);
            tableLayoutPanel5.Controls.Add(tableLayoutPanel7, 1, 0);
            tableLayoutPanel5.Controls.Add(tableLayoutPanel8, 1, 1);
            tableLayoutPanel5.Dock = DockStyle.Fill;
            tableLayoutPanel5.Location = new Point(3, 352);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 2;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.Size = new Size(3173, 375);
            tableLayoutPanel5.TabIndex = 3;
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.ColumnCount = 2;
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel6.Controls.Add(numberOfSequencesLabel, 0, 0);
            tableLayoutPanel6.Controls.Add(label2, 0, 1);
            tableLayoutPanel6.Controls.Add(numberOfSequencesUpDown, 1, 0);
            tableLayoutPanel6.Controls.Add(sequenceActionsUpDown, 1, 1);
            tableLayoutPanel6.Dock = DockStyle.Fill;
            tableLayoutPanel6.Location = new Point(3, 3);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            tableLayoutPanel6.RowCount = 2;
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel6.Size = new Size(874, 181);
            tableLayoutPanel6.TabIndex = 0;
            // 
            // numberOfSequencesLabel
            // 
            numberOfSequencesLabel.AutoSize = true;
            numberOfSequencesLabel.Dock = DockStyle.Fill;
            numberOfSequencesLabel.Location = new Point(3, 0);
            numberOfSequencesLabel.Name = "numberOfSequencesLabel";
            numberOfSequencesLabel.Size = new Size(431, 90);
            numberOfSequencesLabel.TabIndex = 0;
            numberOfSequencesLabel.Text = "Number of Sequences";
            numberOfSequencesLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Location = new Point(3, 90);
            label2.Name = "label2";
            label2.Size = new Size(431, 91);
            label2.TabIndex = 1;
            label2.Text = "Sequence actions";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // numberOfSequencesUpDown
            // 
            numberOfSequencesUpDown.Anchor = AnchorStyles.None;
            numberOfSequencesUpDown.Location = new Point(567, 21);
            numberOfSequencesUpDown.Name = "numberOfSequencesUpDown";
            numberOfSequencesUpDown.Size = new Size(177, 47);
            numberOfSequencesUpDown.TabIndex = 2;
            // 
            // sequenceActionsUpDown
            // 
            sequenceActionsUpDown.Anchor = AnchorStyles.None;
            sequenceActionsUpDown.Location = new Point(570, 112);
            sequenceActionsUpDown.Name = "sequenceActionsUpDown";
            sequenceActionsUpDown.Size = new Size(171, 47);
            sequenceActionsUpDown.TabIndex = 3;
            // 
            // tableLayoutPanel7
            // 
            tableLayoutPanel7.ColumnCount = 2;
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24.0926971F));
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75.9073F));
            tableLayoutPanel7.Controls.Add(protocolLabel, 0, 0);
            tableLayoutPanel7.Controls.Add(comboBox1, 1, 0);
            tableLayoutPanel7.Controls.Add(alwaysCompileProtocolCheckBox, 0, 1);
            tableLayoutPanel7.Controls.Add(button6, 1, 1);
            tableLayoutPanel7.Dock = DockStyle.Fill;
            tableLayoutPanel7.Location = new Point(883, 3);
            tableLayoutPanel7.Name = "tableLayoutPanel7";
            tableLayoutPanel7.RowCount = 2;
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel7.Size = new Size(2287, 181);
            tableLayoutPanel7.TabIndex = 1;
            // 
            // protocolLabel
            // 
            protocolLabel.Anchor = AnchorStyles.None;
            protocolLabel.AutoSize = true;
            protocolLabel.Location = new Point(210, 24);
            protocolLabel.Name = "protocolLabel";
            protocolLabel.Size = new Size(130, 41);
            protocolLabel.TabIndex = 0;
            protocolLabel.Text = "Protocol";
            // 
            // comboBox1
            // 
            comboBox1.Anchor = AnchorStyles.None;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(949, 20);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(940, 49);
            comboBox1.TabIndex = 1;
            // 
            // alwaysCompileProtocolCheckBox
            // 
            alwaysCompileProtocolCheckBox.Anchor = AnchorStyles.None;
            alwaysCompileProtocolCheckBox.AutoSize = true;
            alwaysCompileProtocolCheckBox.Location = new Point(84, 113);
            alwaysCompileProtocolCheckBox.Name = "alwaysCompileProtocolCheckBox";
            alwaysCompileProtocolCheckBox.Size = new Size(382, 45);
            alwaysCompileProtocolCheckBox.TabIndex = 2;
            alwaysCompileProtocolCheckBox.Text = "Always compile protocol";
            alwaysCompileProtocolCheckBox.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            button6.Anchor = AnchorStyles.None;
            button6.Location = new Point(1219, 113);
            button6.Name = "button6";
            button6.Size = new Size(399, 44);
            button6.TabIndex = 3;
            button6.Text = "Edit Protocol";
            button6.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel8
            // 
            tableLayoutPanel8.ColumnCount = 2;
            tableLayoutPanel8.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24.2238731F));
            tableLayoutPanel8.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75.77612F));
            tableLayoutPanel8.Controls.Add(applicationNameLabel, 0, 0);
            tableLayoutPanel8.Controls.Add(applicationVersionLabel, 0, 1);
            tableLayoutPanel8.Controls.Add(applicationNameTextBox, 1, 0);
            tableLayoutPanel8.Controls.Add(applicationVersionTextBox, 1, 1);
            tableLayoutPanel8.Dock = DockStyle.Fill;
            tableLayoutPanel8.Location = new Point(883, 190);
            tableLayoutPanel8.Name = "tableLayoutPanel8";
            tableLayoutPanel8.RowCount = 2;
            tableLayoutPanel8.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel8.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel8.Size = new Size(2287, 182);
            tableLayoutPanel8.TabIndex = 2;
            // 
            // applicationNameLabel
            // 
            applicationNameLabel.Anchor = AnchorStyles.None;
            applicationNameLabel.AutoSize = true;
            applicationNameLabel.Location = new Point(151, 25);
            applicationNameLabel.Name = "applicationNameLabel";
            applicationNameLabel.Size = new Size(250, 41);
            applicationNameLabel.TabIndex = 0;
            applicationNameLabel.Text = "Application name";
            // 
            // applicationVersionLabel
            // 
            applicationVersionLabel.Anchor = AnchorStyles.None;
            applicationVersionLabel.AutoSize = true;
            applicationVersionLabel.Location = new Point(141, 116);
            applicationVersionLabel.Name = "applicationVersionLabel";
            applicationVersionLabel.Size = new Size(271, 41);
            applicationVersionLabel.TabIndex = 1;
            applicationVersionLabel.Text = "Application version";
            // 
            // applicationNameTextBox
            // 
            applicationNameTextBox.Anchor = AnchorStyles.None;
            applicationNameTextBox.Location = new Point(1034, 22);
            applicationNameTextBox.Name = "applicationNameTextBox";
            applicationNameTextBox.Size = new Size(771, 47);
            applicationNameTextBox.TabIndex = 2;
            // 
            // applicationVersionTextBox
            // 
            applicationVersionTextBox.Anchor = AnchorStyles.None;
            applicationVersionTextBox.Location = new Point(1034, 113);
            applicationVersionTextBox.Name = "applicationVersionTextBox";
            applicationVersionTextBox.Size = new Size(771, 47);
            applicationVersionTextBox.TabIndex = 3;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(10, 58);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(3185, 736);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "tabPage2";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(17F, 41F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(3211, 1290);
            Controls.Add(tableLayoutPanel1);
            Name = "MainForm";
            Text = "nTestar";
            WindowState = FormWindowState.Maximized;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tabControlGeneralSettings.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel6.ResumeLayout(false);
            tableLayoutPanel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numberOfSequencesUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)sequenceActionsUpDown).EndInit();
            tableLayoutPanel7.ResumeLayout(false);
            tableLayoutPanel7.PerformLayout();
            tableLayoutPanel8.ResumeLayout(false);
            tableLayoutPanel8.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private Button spyModeButton;
        private Button startInGenerateMode;
        private Button replayModeButton;
        private Button viewModeButton;
        private Button stateModelAnalysisModeButton;
        private TabControl tabControlGeneralSettings;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TableLayoutPanel tableLayoutPanel3;
        private Label label1;
        private TableLayoutPanel tableLayoutPanel4;
        private Button selectSUTButton;
        private ComboBox _sutConnectorComboBox;
        private RichTextBox _sutConnectorTextBox;
        private TableLayoutPanel tableLayoutPanel5;
        private TableLayoutPanel tableLayoutPanel6;
        private Label numberOfSequencesLabel;
        private Label label2;
        private NumericUpDown numberOfSequencesUpDown;
        private NumericUpDown sequenceActionsUpDown;
        private TableLayoutPanel tableLayoutPanel7;
        private Label protocolLabel;
        private ComboBox comboBox1;
        private CheckBox alwaysCompileProtocolCheckBox;
        private Button button6;
        private TableLayoutPanel tableLayoutPanel8;
        private Label applicationNameLabel;
        private Label applicationVersionLabel;
        private TextBox applicationNameTextBox;
        private TextBox applicationVersionTextBox;
    }
}
