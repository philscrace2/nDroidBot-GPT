namespace org.testar.statemodel.@event
{
    // Compatibility namespace matching Java package org.testar.statemodel.event
    public sealed class StateModelEvent
    {
        public StateModelEvent(StateModelEventType eventType, object payload)
        {
            EventType = eventType;
            Payload = payload;
        }

        public StateModelEventType EventType { get; }

        public object Payload { get; }
    }
}
