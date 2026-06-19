using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nTestar.Desktop.Winforms.mvp
{
    public sealed class MainPresenter
    {
        private readonly IMainView _view;
        private readonly MainScreenModel _model;
        private readonly TestarGeneralSettingsSource _settingsSource = new();
        private readonly bool _launchExternalRunner;
        private Process? _activeRunProcess;

        public MainPresenter(IMainView view, MainScreenModel model, bool launchExternalRunner = true)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _launchExternalRunner = launchExternalRunner;
        }

        public void Initialise()
        {
            MainScreenModel effectiveModel = _settingsSource.LoadOrDefault(_model);

            _view.SetProtocols(effectiveModel.Protocols);
            _view.SutConnector = effectiveModel.SutConnector;
            _view.SutConnectorType = effectiveModel.SutConnectorType;
            _view.NumberOfSequences = effectiveModel.NumberOfSequences;
            _view.SequenceActions = effectiveModel.SequenceActions;
            //_view.VisualizeActionsOnGui = _model.VisualizeActionsOnGui;
            _view.AlwaysCompileProtocol = effectiveModel.AlwaysCompileProtocol;
            _view.SelectedProtocol = effectiveModel.Protocol;
            _view.ApplicationName = effectiveModel.ApplicationName;
            _view.ApplicationVersion = effectiveModel.ApplicationVersion;
            //_view.OverrideDisplayScale = _model.OverrideDisplayScale;

            _view.SelectSutRequested += OnSelectSutRequested;
            _view.EditProtocolRequested += OnEditProtocolRequested;
            _view.SpyModeRequested += (s, e) => _view.ShowInfo("SPY mode selected.", "Mode");
            _view.GenerateModeRequested += OnGenerateModeRequested;
            _view.ReplayModeRequested += (s, e) => _view.ShowInfo("Replay mode selected.", "Mode");
            _view.ViewReportRequested += (s, e) => _view.ShowInfo("View report selected.", "Mode");
            _view.ModelModeRequested += (s, e) => _view.ShowInfo("Model visualisation selected.", "Mode");
        }

        private void OnSelectSutRequested(object sender, EventArgs e)
        {
            _view.ShowInfo("Select SUT requested. Wire this to your real SUT selection use case.", "Select SUT");
        }

        private void OnEditProtocolRequested(object sender, EventArgs e)
        {
            _view.ShowInfo("Edit Protocol requested. Wire this to your protocol editor use case.", "Edit Protocol");
        }

        private void OnGenerateModeRequested(object? sender, EventArgs e)
        {
            if (_activeRunProcess != null && !_activeRunProcess.HasExited)
            {
                _view.ShowInfo("A TESTAR run is already active.", "Generate");
                return;
            }

            try
            {
                string protocolSelection = string.IsNullOrWhiteSpace(_view.SelectedProtocol)
                    ? _model.Protocol
                    : _view.SelectedProtocol;

                if (!_launchExternalRunner)
                {
                    string? rootForEmbedded = FindSolutionRoot(AppContext.BaseDirectory);
                    if (string.IsNullOrWhiteSpace(rootForEmbedded))
                    {
                        _view.ShowInfo("Could not resolve solution root for embedded start.", "Generate");
                        return;
                    }

                    string settingsRoot = Path.Combine(rootForEmbedded, "nTestar", "settings");
                    string selectedSse = ResolveExistingSse(NormalizeSseName(protocolSelection), settingsRoot);
                    _settingsSource.SaveGeneralSettings(selectedSse, _view, "Generate");
                    ActivateSseProfile(settingsRoot, selectedSse);

                    if (_view is Form embeddedForm && !embeddedForm.IsDisposed)
                    {
                        embeddedForm.DialogResult = DialogResult.OK;
                        embeddedForm.Close();
                    }

                    return;
                }

                if (!TryResolveNTestarRunner(out string runnerPath, out string runnerDirectory, out bool useDotnetHost))
                {
                    _view.ShowInfo("Could not locate nTestar runner executable. Build nTestar first.", "Generate");
                    return;
                }

                SyncSettingsToRunner(runnerDirectory);
                string sse = ResolveExistingSse(NormalizeSseName(protocolSelection), runnerDirectory);
                _settingsSource.SaveGeneralSettings(sse, _view, "Generate");
                SyncSettingsToRunner(runnerDirectory);
                string arguments = useDotnetHost
                    ? $"\"{Path.GetFileName(runnerPath)}\" sse={sse}"
                    : $"sse={sse}";

                var psi = new ProcessStartInfo
                {
                    FileName = useDotnetHost ? "dotnet" : runnerPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    WorkingDirectory = runnerDirectory
                };

                var process = Process.Start(psi);
                if (process == null)
                {
                    _view.ShowInfo("Failed to start TESTAR process.", "Generate");
                    return;
                }

                _activeRunProcess = process;
                process.EnableRaisingEvents = true;
                process.Exited += (_, _) =>
                {
                    _activeRunProcess = null;
                    if (_view is Form form && !form.IsDisposed)
                    {
                        try
                        {
                            form.BeginInvoke(new Action(form.Show));
                        }
                        catch
                        {
                            // Ignore UI race during app shutdown.
                        }
                    }
                };

                if (_view is Form launchForm && !launchForm.IsDisposed)
                {
                    launchForm.Hide();
                }
            }
            catch (Exception ex)
            {
                _view.ShowInfo($"Failed to start Generate mode: {ex.Message}", "Generate");
            }
        }

        private static string NormalizeSseName(string protocolSelection)
        {
            string trimmed = (protocolSelection ?? string.Empty).Trim();
            if (trimmed.EndsWith("_protocol", StringComparison.OrdinalIgnoreCase))
            {
                trimmed = trimmed[..^"_protocol".Length];
            }

            return string.IsNullOrWhiteSpace(trimmed) ? "desktop_generic" : trimmed;
        }

        private static string ResolveExistingSse(string requestedSse, string runnerDirectory)
        {
            string settingsDir = Path.Combine(runnerDirectory, "settings");
            if (!Directory.Exists(settingsDir))
            {
                return requestedSse;
            }

            if (Directory.Exists(Path.Combine(settingsDir, requestedSse)))
            {
                return requestedSse;
            }

            if (Directory.Exists(Path.Combine(settingsDir, "desktop_generic")))
            {
                return "desktop_generic";
            }

            string? firstValid = Directory.GetDirectories(settingsDir)
                .FirstOrDefault(d => File.Exists(Path.Combine(d, "test.testarsettings")));
            if (!string.IsNullOrWhiteSpace(firstValid))
            {
                return Path.GetFileName(firstValid);
            }

            return requestedSse;
        }

        private static void ActivateSseProfile(string settingsDir, string sse)
        {
            if (!Directory.Exists(settingsDir))
            {
                return;
            }

            foreach (string existing in Directory.GetFiles(settingsDir, "*.sse"))
            {
                File.Delete(existing);
            }

            File.WriteAllText(Path.Combine(settingsDir, sse + ".sse"), string.Empty);
        }

        private static bool TryResolveNTestarRunner(out string runnerPath, out string runnerDirectory, out bool useDotnetHost)
        {
            runnerPath = string.Empty;
            runnerDirectory = string.Empty;
            useDotnetHost = false;
            string? root = FindSolutionRoot(AppContext.BaseDirectory);
            if (string.IsNullOrWhiteSpace(root))
            {
                return false;
            }

            string[] candidateBaseDirs =
            {
                Path.Combine(root, "Debug"),
                Path.Combine(root, "nTestar", "bin", "Debug", "net8.0-windows"),
                Path.Combine(root, "nTestar", "bin", "Release", "net8.0-windows")
            };

            string[] buildKinds = { "Debug", "Release" };
            string[] runnerFileNames = { "nTestar.Desktop.Console.exe", "nTestar.Desktop.Console", "Console.nTestar.dll" };

            foreach (string baseDir in candidateBaseDirs)
            {
                foreach (string fileName in runnerFileNames)
                {
                    string candidate = Path.Combine(baseDir, fileName);
                    if (File.Exists(candidate))
                    {
                        string depsPath = Path.Combine(baseDir, "Console.nTestar.deps.json");
                        string depsContent = File.Exists(depsPath) ? File.ReadAllText(depsPath) : string.Empty;
                        bool depsLooksValid = string.IsNullOrEmpty(depsContent)
                            || depsContent.Contains("Core.nTestar", StringComparison.Ordinal)
                            || depsContent.Contains("Testar.Core", StringComparison.Ordinal)
                            || depsContent.Contains("nTestar.Core", StringComparison.Ordinal);
                        if (!depsLooksValid)
                        {
                            continue;
                        }

                        runnerPath = candidate;
                        runnerDirectory = baseDir;
                        useDotnetHost = candidate.EndsWith(".dll", StringComparison.OrdinalIgnoreCase);
                        return true;
                    }
                }
            }

            // Fallback for non-unified output directories if needed.
            foreach (string buildKind in buildKinds)
            {
                string baseDir = Path.Combine(root, "nTestar", "bin", buildKind, "net8.0-windows");
                foreach (string fileName in runnerFileNames)
                {
                    string candidate = Path.Combine(baseDir, fileName);
                    if (!File.Exists(candidate))
                    {
                        continue;
                    }

                    runnerPath = candidate;
                    runnerDirectory = baseDir;
                    useDotnetHost = candidate.EndsWith(".dll", StringComparison.OrdinalIgnoreCase);
                    return true;
                }
            }

            return false;
        }

        private static string? FindSolutionRoot(string startDirectory)
        {
            var directory = new DirectoryInfo(startDirectory);
            while (directory != null)
            {
                string sln = Path.Combine(directory.FullName, "nDroidBot-GPT.sln");
                if (File.Exists(sln))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }

            return null;
        }

        private static void SyncSettingsToRunner(string runnerDirectory)
        {
            string? root = FindSolutionRoot(AppContext.BaseDirectory);
            if (string.IsNullOrWhiteSpace(root))
            {
                return;
            }

            string sourceSettings = Path.Combine(root, "nTestar", "settings");
            string targetSettings = Path.Combine(runnerDirectory, "settings");
            if (!Directory.Exists(sourceSettings))
            {
                return;
            }

            if (Directory.Exists(targetSettings))
            {
                Directory.Delete(targetSettings, true);
            }

            CopyDirectory(sourceSettings, targetSettings);
        }

        private static void CopyDirectory(string sourceDirectory, string targetDirectory)
        {
            Directory.CreateDirectory(targetDirectory);

            foreach (string file in Directory.GetFiles(sourceDirectory))
            {
                string destinationFile = Path.Combine(targetDirectory, Path.GetFileName(file));
                File.Copy(file, destinationFile, true);
            }

            foreach (string directory in Directory.GetDirectories(sourceDirectory))
            {
                string destinationDir = Path.Combine(targetDirectory, Path.GetFileName(directory));
                CopyDirectory(directory, destinationDir);
            }
        }
    }

    // -----------------------------
    // Humble control black box
    // -----------------------------

    public sealed class ModeButtonState
    {
        public string Caption { get; }
        public string ToolTipText { get; }
        public Color BorderColor { get; }
        public Color BackgroundColor { get; }
        public Color CaptionColor { get; internal set; }

        public ModeButtonState(string caption, string toolTipText, Color borderColor, Color backgroundColor)
        {
            Caption = caption ?? string.Empty;
            ToolTipText = toolTipText ?? string.Empty;
            BorderColor = borderColor;
            BackgroundColor = backgroundColor;
        }
    }

    public sealed class ModeButtonBlackBox
    {
        private readonly string _caption;
        private readonly string _toolTipText;
        private bool _isHovering;

        public ModeButtonBlackBox(string caption, string toolTipText)
        {
            _caption = caption ?? string.Empty;
            _toolTipText = toolTipText ?? string.Empty;
        }

        public void PointerEntered() => _isHovering = true;
        public void PointerLeft() => _isHovering = false;

        public ModeButtonState GetState()
        {
            return new ModeButtonState(
                _caption,
                _toolTipText,
                _isHovering ? Color.DodgerBlue : Color.SlateGray,
                Color.White);
        }
    }

    public sealed class ModeButtonControl : Control
    {
        private readonly ModeButtonBlackBox _logic;
        private readonly ToolTip _toolTip;
        private Image _image;

        public ModeButtonControl(string caption, string tooltip, Image image)
        {
            _logic = new ModeButtonBlackBox(caption, tooltip);
            _image = image;
            _toolTip = new ToolTip();

            Width = 224;
            Height = 244;
            Cursor = Cursors.Hand;
            DoubleBuffered = true;
            Margin = new Padding(6);

            var state = _logic.GetState();
            _toolTip.SetToolTip(this, state.ToolTipText);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _logic.PointerEntered();
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _logic.PointerLeft();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var state = _logic.GetState();
            using (var background = new SolidBrush(state.BackgroundColor))
            {
                e.Graphics.FillRectangle(background, ClientRectangle);
            }

            using (var border = new Pen(state.BorderColor))
            {
                e.Graphics.DrawRectangle(border, 0, 0, Width - 1, Height - 1);
            }

            if (_image != null)
            {
                e.Graphics.DrawImage(_image, new Rectangle(12, 12, 200, 200));
            }

            using (var captionBrush = new SolidBrush(state.CaptionColor))
            {
                e.Graphics.DrawString(state.Caption, Font, captionBrush, new PointF(12, 220));
            }
        }
    }

}
