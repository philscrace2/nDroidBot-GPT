namespace org.testar.statemodel.events
{
    public interface StateModelEventListener
    {
        void EventReceived(StateModelEvent modelEvent);

        void SetListening(bool listening);
    }
}
