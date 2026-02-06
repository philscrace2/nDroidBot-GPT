using System.Collections.Generic;
using org.testar.monkey;
using org.testar.monkey.alayer;
using org.testar.monkey.alayer.devices;

namespace org.testar.monkey.alayer.actions
{
    public class StdActionCompiler
    {
        protected readonly Abstractor abstractor;
        private readonly Action lMouseDown = new MouseDown(MouseButtons.BUTTON1);
        private readonly Action rMouseDown = new MouseDown(MouseButtons.BUTTON3);
        private readonly Action lMouseUp = new MouseUp(MouseButtons.BUTTON1);
        private readonly Action rMouseUp = new MouseUp(MouseButtons.BUTTON3);
        private readonly Action nop = new NOP();

        public StdActionCompiler() : this(new StdAbstractor())
        {
        }

        public StdActionCompiler(Abstractor abstractor)
        {
            this.abstractor = abstractor;
        }

        public virtual Action mouseMove(Widget widget)
        {
            Finder wf = abstractor.apply(widget);
            Position position = new WidgetPosition(wf, Tags.Shape, 0.5, 0.5, true);
            position.obscuredByChildFeature(false);
            Action ret = mouseMove(widget, position);
            ret.mapOriginWidget(widget);
            return ret;
        }

        public virtual Action mouseMove(Widget widget, Position position)
        {
            return new MouseMove(position);
        }

        public virtual Action leftClick()
        {
            return new CompoundAction.Builder()
                .add(lMouseDown, 0)
                .add(lMouseUp, 0)
                .add(nop, 1)
                .build();
        }

        public virtual Action rightClick()
        {
            return new CompoundAction.Builder()
                .add(rMouseDown, 0)
                .add(rMouseUp, 0)
                .add(nop, 1)
                .build();
        }

        public virtual Action leftClickAt(Position position)
        {
            Assert.notNull(position);
            return new CompoundAction.Builder()
                .add(new MouseMove(position), 1)
                .add(lMouseDown, 0)
                .add(lMouseUp, 0)
                .build();
        }

        public virtual Action leftClickAt(Widget widget)
        {
            return leftClickAt(widget, 0.5, 0.5);
        }

        public virtual Action leftClickAt(Widget widget, double relX, double relY)
        {
            Finder wf = abstractor.apply(widget);
            Action ret = leftClickAt(new WidgetPosition(wf, Tags.Shape, relX, relY, true));
            var targets = Util.newArrayList<Finder>();
            targets.Add(wf);
            ret.set(Tags.Targets, targets);
            ret.set(Tags.TargetID, widget.get(Tags.ConcreteID));
            ret.mapOriginWidget(widget);
            return ret;
        }

        public virtual Action slideFromTo(Position from, Position to)
        {
            Action action = dragFromTo(from, to);
            action.set(Tags.Slider, new Position[] { from, to });
            return action;
        }

        public virtual Action slideFromTo(Position from, Position to, Widget widget)
        {
            Action action = slideFromTo(from, to);
            action.mapOriginWidget(widget);
            return action;
        }

        public virtual Action dragFromTo(Position from, Position to)
        {
            return new CompoundAction.Builder()
                .add(new MouseMove(from), 1)
                .add(lMouseDown, 0)
                .add(new MouseMove(to), 1)
                .add(lMouseUp, 0)
                .build();
        }

        public virtual Action clickTypeInto(Widget widget, string text, bool replaceText)
        {
            return clickTypeInto(widget, 0.5, 0.5, text, replaceText);
        }

        public virtual Action clickTypeInto(Widget widget, double relX, double relY, string text, bool replaceText)
        {
            Finder wf = abstractor.apply(widget);
            Action ret = replaceText
                ? clickAndReplaceText(new WidgetPosition(wf, Tags.Shape, relX, relY, true), text)
                : clickAndAppendText(new WidgetPosition(wf, Tags.Shape, relX, relY, true), text);

            var targets = Util.newArrayList<Finder>();
            targets.Add(wf);
            ret.set(Tags.Targets, targets);
            ret.set(Tags.TargetID, widget.get(Tags.ConcreteID));
            return ret;
        }

        public virtual Action clickAndReplaceText(Position position, string text)
        {
            Assert.notNull(position, text);
            var builder = new CompoundAction.Builder()
                .add(leftClickAt(position), 1)
                .add(new KeyDown(KBKeys.VK_CONTROL), 0.1)
                .add(new KeyDown(KBKeys.VK_A), 0.1)
                .add(new KeyUp(KBKeys.VK_A), 0.1)
                .add(new KeyUp(KBKeys.VK_CONTROL), 0.1)
                .add(new Type(text), 1);
            return builder.build();
        }

        public virtual Action clickAndAppendText(Position position, string text)
        {
            Assert.notNull(position, text);
            var builder = new CompoundAction.Builder()
                .add(leftClickAt(position), 1)
                .add(new KeyDown(KBKeys.VK_END), 0.1)
                .add(new KeyUp(KBKeys.VK_END), 0.1)
                .add(new Type(text), 1);
            return builder.build();
        }

        public virtual Action pasteTextInto(Widget widget, string text, bool replaceText)
        {
            return pasteTextInto(widget, 0.5, 0.5, text, replaceText);
        }

        public virtual Action pasteTextInto(Widget widget, double relX, double relY, string text, bool replaceText)
        {
            Finder wf = abstractor.apply(widget);
            Action ret = replaceText
                ? pasteAndReplaceText(new WidgetPosition(wf, Tags.Shape, relX, relY, true), text)
                : pasteAndAppendText(new WidgetPosition(wf, Tags.Shape, relX, relY, true), text);

            var targets = Util.newArrayList<Finder>();
            targets.Add(wf);
            ret.set(Tags.Targets, targets);
            ret.set(Tags.TargetID, widget.get(Tags.ConcreteID));
            ret.mapOriginWidget(widget);
            return ret;
        }

        public virtual Action pasteAndReplaceText(Position position, string text)
        {
            Assert.notNull(position, text);
            var builder = new CompoundAction.Builder()
                .add(leftClickAt(position), 1)
                .add(new KeyDown(KBKeys.VK_CONTROL), 0.1)
                .add(new KeyDown(KBKeys.VK_A), 0.1)
                .add(new KeyUp(KBKeys.VK_A), 0.1)
                .add(new KeyUp(KBKeys.VK_CONTROL), 0.1)
                .add(new PasteText(text), 1);
            return builder.build();
        }

        public virtual Action pasteAndAppendText(Position position, string text)
        {
            Assert.notNull(position, text);
            var builder = new CompoundAction.Builder()
                .add(leftClickAt(position), 1)
                .add(new KeyDown(KBKeys.VK_END), 0.1)
                .add(new KeyUp(KBKeys.VK_END), 0.1)
                .add(new PasteText(text), 1);
            return builder.build();
        }

        public Action hitKey(KBKeys key)
        {
            return new CompoundAction.Builder()
                .add(new KeyDown(key), 0)
                .add(new KeyUp(key), 1)
                .add(nop, 1)
                .build();
        }

        public Action hitShortcutKey(List<KBKeys> keys)
        {
            if (keys.Count == 1)
            {
                return hitKey(keys[0]);
            }

            var builder = new CompoundAction.Builder();
            for (int i = 0; i < keys.Count; i++)
            {
                builder.add(new KeyDown(keys[i]), i == 0 ? 0 : 0.1);
            }

            for (int i = keys.Count - 1; i >= 0; i--)
            {
                builder.add(new KeyUp(keys[i]), i == keys.Count - 1 ? 1.0 : 0);
            }

            builder.add(nop, 1.0);
            return builder.build();
        }
    }
}
