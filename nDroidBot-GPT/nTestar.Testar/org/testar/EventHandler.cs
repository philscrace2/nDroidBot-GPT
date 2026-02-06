namespace org.testar
{
    public class EventHandler
    {
        private readonly IEventListener listener;

        public EventHandler(IEventListener listener)
        {
            this.listener = listener;
        }

        public void KeyDown(object key)
        {
            // TODO: Wire platform-specific native hooks.
        }
    }
}
