namespace org.testar.monkey.alayer.devices
{
    public interface Keyboard
    {
        void press(KBKeys key);
        void release(KBKeys key);
        void paste();
    }
}
