using org.testar;
using org.testar.managers;
using org.testar.monkey;
using org.testar.monkey.alayer;
using org.testar.monkey.alayer.devices;
using org.testar.settings;
using Action = org.testar.monkey.alayer.Action;

namespace org.testar.protocols
{
    public class ClickFilterLayerProtocol : org.testar.monkey.DefaultProtocol
    {
        private bool preciseCoding;
        private bool displayWhiteTabu;
        private bool whiteTabuMode;
        private bool shiftPressed;

        private double mouseX = double.MinValue;
        private double mouseY = double.MinValue;
        private readonly double[] filterArea = { double.MaxValue, double.MaxValue, double.MinValue, double.MinValue };

        private readonly FilteringManager filteringManager;

        public ClickFilterLayerProtocol() : base(new Settings())
        {
            filteringManager = new FilteringManager();
            filteringManager.loadFilters();
            displayWhiteTabu = false;
        }

        public override void keyDown(KBKeys key)
        {
            base.keyDown(key);
            if (mode() == Modes.Spy)
            {
                if (key == KBKeys.VK_CAPS_LOCK || key == KBKeys.VK_ALT)
                {
                    displayWhiteTabu = !displayWhiteTabu;
                }
                else if (key == KBKeys.VK_TAB)
                {
                    preciseCoding = !preciseCoding;
                }
                else if (key == KBKeys.VK_SHIFT)
                {
                    shiftPressed = true;
                }
                else if (key == KBKeys.VK_CONTROL)
                {
                    filterArea[0] = mouseX;
                    filterArea[1] = mouseY;
                }
            }
        }

        public override void keyUp(KBKeys key)
        {
            base.keyUp(key);
            if (mode() == Modes.Spy)
            {
                if (key == KBKeys.VK_SHIFT)
                {
                    shiftPressed = false;
                }
                else if (key == KBKeys.VK_CONTROL && displayWhiteTabu)
                {
                    filterArea[2] = mouseX;
                    filterArea[3] = mouseY;
                    whiteTabuMode = shiftPressed;
                    filteringManager.manageWhiteTabuLists(getStateForClickFilterLayerProtocol(), mouse, filterArea, whiteTabuMode, preciseCoding);
                }
            }
        }

        public override void mouseMoved(double x, double y)
        {
            mouseX = x;
            mouseY = y;
        }

        protected override void visualizeActions(Canvas canvas, State state, System.Collections.Generic.ISet<Action> actions)
        {
            SutVisualization.visualizeActions(canvas, state, actions);
            if (displayWhiteTabu && mode() == Modes.Spy)
            {
                filteringManager.visualizeActions(canvas, state);
            }
        }

        protected bool blackListed(Widget widget)
        {
            return filteringManager.blackListed(widget);
        }

        protected bool whiteListed(Widget widget)
        {
            return filteringManager.whiteListed(widget);
        }
    }
}
