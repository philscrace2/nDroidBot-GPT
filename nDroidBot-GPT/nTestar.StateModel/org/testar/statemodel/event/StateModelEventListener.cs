namespace org.testar.statemodel.event
{
    public interface StateModelEventListener
    {
        void EventReceived(StateModelEvent modelEvent);

        void SetListening(bool listening);
    }
}
