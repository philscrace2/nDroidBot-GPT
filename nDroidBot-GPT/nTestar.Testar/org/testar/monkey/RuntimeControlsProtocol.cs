using System;
using System.Collections.Generic;
using org.testar.settings;
using org.testar.monkey.alayer.devices;

namespace org.testar.monkey
{
    public abstract class RuntimeControlsProtocol : AbstractProtocol, org.testar.IEventListener
    {
        public double Delay { get; private set; } = double.MinValue;
        public object[]? UserEvent { get; private set; }
        public bool MarkParentWidget { get; private set; }
        public bool VisualizationOn { get; private set; }
        public Modes Mode { get; set; }

        private readonly HashSet<KBKeys> pressed = new HashSet<KBKeys>();

        public Settings Settings { get; }

        protected RuntimeControlsProtocol(Settings settings)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.settings = settings;
        }

        public org.testar.EventHandler InitializeEventHandler()
        {
            return new org.testar.EventHandler(this);
        }

        protected void setMode(Modes mode)
        {
            if (Mode == mode)
            {
                return;
            }

            Mode = mode;
        }

        public Modes mode()
        {
            return Mode;
        }

        public virtual void keyDown(KBKeys key)
        {
            if (Settings.Get("KeyBoardListener", "false") == null)
            {
                pressed.Add(key);

                if (pressed.Contains(KBKeys.VK_SHIFT) && key == KBKeys.VK_SPACE)
                {
                    if (Delay == double.MinValue)
                    {
                        Delay = double.Parse(Settings.Get("TimeToWaitAfterAction", "0.0"));
                        Settings.Set("TimeToWaitAfterAction", "SLOW_MOTION");
                    }
                    else
                    {
                        Settings.Set("TimeToWaitAfterAction", Delay.ToString());
                        Delay = double.MinValue;
                    }
                }
                else if (key == KBKeys.VK_DOWN && pressed.Contains(KBKeys.VK_SHIFT))
                {
                    Mode = Modes.Quit;
                }
                else if (key == KBKeys.VK_UP && pressed.Contains(KBKeys.VK_SHIFT))
                {
                    VisualizationOn = !VisualizationOn;
                }
                else if (key == KBKeys.VK_0 && pressed.Contains(KBKeys.VK_SHIFT))
                {
                    Environment.SetEnvironmentVariable("DEBUG_WINDOWS_PROCESS_NAMES", "true");
                }
                else if (!pressed.Contains(KBKeys.VK_SHIFT) && Mode == Modes.Record && UserEvent == null)
                {
                    UserEvent = new object[] { key };
                }
                else if (pressed.Contains(KBKeys.VK_ALT) && pressed.Contains(KBKeys.VK_SHIFT))
                {
                    MarkParentWidget = !MarkParentWidget;
                }
            }
        }

        public virtual void keyUp(KBKeys key)
        {
            if (Settings.Get("KeyBoardListener", "false") == null)
            {
                pressed.Remove(key);
            }
        }

        public virtual void mouseDown(MouseButtons btn, double x, double y)
        {
        }

        public virtual void mouseUp(MouseButtons btn, double x, double y)
        {
            if (Mode == Modes.Record && UserEvent == null)
            {
                UserEvent = new object[] { btn, x, y };
            }
        }

        public virtual void mouseMoved(double x, double y)
        {
        }
    }

    public enum Modes
    {
        Spy,
        Record,
        Generate,
        Quit,
        View,
        Replay
    }
}
