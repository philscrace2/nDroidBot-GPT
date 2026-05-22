using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nTestar.Desktop.Winforms.mvp
{
    public sealed class MainPresenter
    {
        private readonly IMainView _view;
        private readonly MainScreenModel _model;

        public MainPresenter(IMainView view, MainScreenModel model)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public void Initialise()
        {
            _view.SetProtocols(_model.Protocols);
            _view.SutConnector = _model.SutConnector;
            _view.NumberOfSequences = _model.NumberOfSequences;
            _view.SequenceActions = _model.SequenceActions;
            //_view.VisualizeActionsOnGui = _model.VisualizeActionsOnGui;
            _view.AlwaysCompileProtocol = _model.AlwaysCompileProtocol;
            //_view.SelectedProtocol = _model.Protocol;
            //_view.ApplicationName = _model.ApplicationName;
            //_view.ApplicationVersion = _model.ApplicationVersion;
            //_view.OverrideDisplayScale = _model.OverrideDisplayScale;

            _view.SelectSutRequested += OnSelectSutRequested;
            _view.EditProtocolRequested += OnEditProtocolRequested;
            _view.SpyModeRequested += (s, e) => _view.ShowInfo("SPY mode selected.", "Mode");
            _view.GenerateModeRequested += (s, e) => _view.ShowInfo("Generate mode selected.", "Mode");
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
