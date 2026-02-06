using org.testar.monkey;
using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.actions
{
    public class AnnotatingActionCompiler : StdActionCompiler
    {
        private const int DisplayTextMaxLength = 16;

        public override Action leftClickAt(Widget widget)
        {
            var action = base.leftClickAt(widget);
            action.set(Tags.Desc, "Left Click at '" + widget.get(Tags.Desc, "<no description>") + "'");
            action.set(Tags.Role, ActionRoles.LeftClickAt);
            action.mapOriginWidget(widget);
            return action;
        }

        public override Action clickTypeInto(Widget widget, string textToType, bool replaceText)
        {
            var action = base.clickTypeInto(widget, textToType, replaceText);
            action.set(Tags.Desc, "Type '" + Util.abbreviate(textToType, DisplayTextMaxLength, "...") + "' into '" +
                                  widget.get(Tags.Desc, "<no description>") + "'");
            action.set(Tags.Role, ActionRoles.ClickTypeInto);
            action.mapOriginWidget(widget);
            return action;
        }

        public override Action pasteTextInto(Widget widget, string textToPaste, bool replaceText)
        {
            var action = base.pasteTextInto(widget, textToPaste, replaceText);
            action.set(Tags.Desc, "Paste Text: " + Util.abbreviate(textToPaste, DisplayTextMaxLength, "..."));
            action.set(Tags.Role, ActionRoles.PasteTextInto);
            action.mapOriginWidget(widget);
            return action;
        }

        public override Action slideFromTo(Position from, Position to, Widget widget)
        {
            var action = base.slideFromTo(from, to, widget);
            action.set(Tags.Role, ActionRoles.LeftDrag);
            return action;
        }
    }
}
