using System.Collections.Generic;
using org.testar;
using org.testar.environment;
using org.testar.managers;
using org.testar.monkey.alayer;
using org.testar.monkey.alayer.actions;
using org.testar.monkey.alayer.windows;
using org.testar.monkey;
using Action = org.testar.monkey.alayer.Action;
using Environment = org.testar.environment.Environment;

namespace org.testar.protocols
{
    public class DesktopProtocol : GenericUtilsProtocol
    {
        protected override void beginSequence(SUT system, State state)
        {
            base.beginSequence(system, state);

            if (state.childCount() == 0)
            {
                return;
            }

            double displayScale = Environment.getInstance().GetDisplayScale(state.child(0).get(Tags.HWND, 0L));
            mouse?.setCursorDisplayScale(displayScale);
        }

        protected override State getState(SUT system)
        {
            State state = base.getState(system);

            if (state.childCount() > 0 && state.child(0).get(UIATags.UIAFrameworkId, string.Empty) == "Qt")
            {
                Widget modalWindow = state.child(0);
                foreach (var widget in state)
                {
                    if (widget.get(UIATags.UIAIsWindowModal, false))
                    {
                        modalWindow = widget;
                        break;
                    }
                }

                if (modalWindow != null)
                {
                    foreach (var widget in state)
                    {
                        if (!isQtElementVisibleOnModalScreen(widget, modalWindow))
                        {
                            widget.set(Tags.Blocked, true);
                        }
                    }
                }
            }

            return state;
        }

        private static bool isQtElementVisibleOnModalScreen(Widget widget, Widget modalWidget)
        {
            var elementRect = widget.get(Tags.Shape, Rect.from(0, 0, 0, 0));
            var modalRect = modalWidget.get(Tags.Shape, Rect.from(0, 0, 0, 0));

            double elementRight = elementRect.x() + elementRect.width();
            double elementBottom = elementRect.y() + elementRect.height();
            double modalRight = modalRect.x() + modalRect.width();
            double modalBottom = modalRect.y() + modalRect.height();

            return elementRect.x() >= modalRect.x() &&
                   elementRight <= modalRight &&
                   elementRect.y() >= modalRect.y() &&
                   elementBottom <= modalBottom;
        }

        protected override ISet<Action> deriveActions(SUT system, State state)
        {
            return base.deriveActions(system, state);
        }

        protected virtual DerivedActions deriveClickTypeScrollActionsFromAllWidgets(ISet<Action> actions, State state)
        {
            var filteredActions = new HashSet<Action>();
            var derived = new DerivedActions(actions, filteredActions);

            foreach (var widget in state)
            {
                derived = getMultipleActionsFromWidget(widget, derived);
            }

            return derived;
        }

        protected virtual DerivedActions deriveClickTypeScrollActionsFromTopLevelWidgets(ISet<Action> actions, State state)
        {
            var filteredActions = new HashSet<Action>();
            var derived = new DerivedActions(actions, filteredActions);

            foreach (var widget in getTopWidgets(state))
            {
                derived = getMultipleActionsFromWidget(widget, derived);
            }

            return derived;
        }

        private DerivedActions getMultipleActionsFromWidget(Widget widget, DerivedActions derived)
        {
            StdActionCompiler ac = new AnnotatingActionCompiler();

            if (widget.get(Tags.Role, Roles.Widget).ToString()?.Equals("UIAMenu", System.StringComparison.OrdinalIgnoreCase) == true)
            {
                return derived;
            }

            if (!widget.get(Tags.Enabled, true) || widget.get(Tags.Blocked, false))
            {
                return derived;
            }

            if (blackListed(widget))
            {
                if (isTypeable(widget))
                {
                    derived.addFilteredAction(ac.clickTypeInto(widget, InputDataManager.getRandomTextInputData(widget), true));
                }
                else
                {
                    derived.addFilteredAction(ac.leftClickAt(widget));
                }

                return derived;
            }

            if (isTypeable(widget))
            {
                if (isUnfiltered(widget) || whiteListed(widget))
                {
                    derived.addAvailableAction(ac.clickTypeInto(widget, InputDataManager.getRandomTextInputData(widget), true));
                }
                else
                {
                    derived.addFilteredAction(ac.clickTypeInto(widget, InputDataManager.getRandomTextInputData(widget), true));
                }
            }

            if (isClickable(widget))
            {
                if (isUnfiltered(widget) || whiteListed(widget))
                {
                    derived.addAvailableAction(ac.leftClickAt(widget));
                }
                else
                {
                    derived.addFilteredAction(ac.leftClickAt(widget));
                }
            }

            var drags = widget.scrollDrags(SCROLL_ARROW_SIZE, SCROLL_THICK);
            if (drags != null)
            {
                derived = addSlidingActions(derived, ac, drags, widget);
            }

            return derived;
        }
    }
}
