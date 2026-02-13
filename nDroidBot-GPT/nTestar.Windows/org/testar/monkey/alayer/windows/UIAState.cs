using org.testar.monkey.alayer;
using org.testar.stub;

namespace org.testar.monkey.alayer.windows
{
    public sealed class UIAState : StateStub
    {
        private static readonly IReadOnlyCollection<ITag> RootFetchableTags = new ITag[]
        {
            Tags.PID,
            Tags.IsRunning,
            Tags.TimeStamp,
            Tags.Foreground,
            Tags.HasStandardMouse
        };

        internal UIARootElement? RootElement { get; private set; }

        internal void SetRootElement(UIARootElement rootElement)
        {
            RootElement = rootElement;
        }

        public UIAWidget addChild(UIAWidget child)
        {
            child.setParent(this);
            addChild((Widget)child);
            return child;
        }

        protected override T fetch<T>(Tag<T> tag)
        {
            object? value = ResolveRootTagValue(tag);
            if (value is T typed)
            {
                return typed;
            }

            return default!;
        }

        protected override IEnumerable<ITag> tagDomain()
        {
            return RootFetchableTags;
        }

        private object? ResolveRootTagValue(ITag tag)
        {
            UIARootElement? root = RootElement;
            if (root == null)
            {
                return null;
            }

            if (tag == Tags.PID)
            {
                return root.Pid;
            }

            if (tag == Tags.IsRunning)
            {
                return root.IsRunning;
            }

            if (tag == Tags.TimeStamp)
            {
                return root.TimeStamp;
            }

            if (tag == Tags.Foreground)
            {
                return root.IsForeground;
            }

            if (tag == Tags.HasStandardMouse)
            {
                return root.HasStandardMouse;
            }

            return null;
        }
    }
}
