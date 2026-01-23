using System;
using System.Collections.Generic;
using Core.nTestar.Devices;
using BaseSettings = Core.nTestar.Base.Settings;

namespace Core.nTestar
{
    public abstract class RuntimeControlsProtocol : AbstractProtocol, IEventListener
    {
        public double Delay { get; private set; } = double.MinValue;
        public object[] UserEvent { get; private set; } = null;
        public bool MarkParentWidget { get; private set; } = false;
        public bool VisualizationOn { get; private set; } = false;



        public Modes Mode { get; set; }
        private HashSet<KBKeys> pressed = new HashSet<KBKeys>();

        public BaseSettings Settings { get; }

        public RuntimeControlsProtocol(BaseSettings settings)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public EventHandler InitializeEventHandler()
        {
            return new EventHandler(this);
        }

        protected void SetMode(Modes mode)
        {
            if (Mode == mode) return;
            Mode = mode;
        }

        private const double SLOW_MOTION = 2.0;

        public void KeyDown(KBKeys key)
        {
            if (Settings.Get("KeyBoardListener", "false") == null)
            {
                pressed.Add(key);

                if (pressed.Contains(KBKeys.VK_SHIFT) && key == KBKeys.VK_SPACE)
                {
                    if (Delay == double.MinValue)
                    {
                        Delay = Double.Parse(Settings.Get("TimeToWaitAfterAction", "0.0"));
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
                    System.Environment.SetEnvironmentVariable("DEBUG_WINDOWS_PROCESS_NAMES", "true");
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

        public void KeyUp(KBKeys key)
        {
            if (Settings.Get("KeyBoardListener", "false") == null)
            {
                pressed.Remove(key);
            }
        }

        public void MouseDown(MouseButtons btn, double x, double y) { }

        public void MouseUp(MouseButtons btn, double x, double y)
        {
            if (Mode == Modes.Record && UserEvent == null)
            {
                UserEvent = new object[] { btn, x, y };
            }
        }

        public void MouseMoved(double x, double y) { }
    }


    public class MouseButtons { }
    public class EventHandler
    {
        public EventHandler(IEventListener listener) { }
    }
    public interface IEventListener { }

    public enum Modes { Spy, Record, Generate, Quit, View, Replay }


}
