using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.actions
{
    public class AnnotatingActionCompiler : StdActionCompiler
    {
        public Action leftClickAt(Widget widget)
        {
            var action = new NOP();
            action.mapOriginWidget(widget);
            return action;
        }

        public Action clickTypeInto(Widget widget, string textToType, bool replaceText)
        {
            var action = new NOP();
            action.mapOriginWidget(widget);
            action.set(Tags.InputText, textToType);
            return action;
        }

        public Action pasteTextInto(Widget widget, string textToPaste, bool replaceText)
        {
            var action = new NOP();
            action.mapOriginWidget(widget);
            action.set(Tags.InputText, textToPaste);
            return action;
        }

        public Action slideFromTo(AbsolutePosition from, AbsolutePosition to, Widget widget)
        {
            var action = new NOP();
            action.mapOriginWidget(widget);
            return action;
        }
    }
}
