using org.testar.statemodel.events;
using org.testar.statemodel.exceptions;
using org.testar.statemodel.sequence;

namespace org.testar.statemodel.util
{
    public class EventHelper
    {
        public void ValidateEvent(StateModelEvent modelEvent)
        {
            if (modelEvent == null || modelEvent.Payload == null)
            {
                throw new InvalidEventException();
            }

            switch (modelEvent.EventType)
            {
                case StateModelEventType.AbstractStateAdded:
                case StateModelEventType.AbstractStateChanged:
                    if (modelEvent.Payload is not AbstractState)
                    {
                        throw new InvalidEventException();
                    }
                    break;

                case StateModelEventType.AbstractStateTransitionAdded:
                case StateModelEventType.AbstractActionChanged:
                    if (modelEvent.Payload is not AbstractStateTransition)
                    {
                        throw new InvalidEventException();
                    }
                    break;

                case StateModelEventType.AbstractStateModelInitialized:
                    if (modelEvent.Payload is not AbstractStateModel)
                    {
                        throw new InvalidEventException();
                    }
                    break;

                case StateModelEventType.SequenceStarted:
                case StateModelEventType.SequenceEnded:
                    if (modelEvent.Payload is not Sequence)
                    {
                        throw new InvalidEventException();
                    }
                    break;

                case StateModelEventType.SequenceManagerInitialized:
                    if (modelEvent.Payload is not SequenceManager)
                    {
                        throw new InvalidEventException();
                    }
                    break;

                case StateModelEventType.SequenceNodeAdded:
                case StateModelEventType.SequenceNodeUpdated:
                    if (modelEvent.Payload is not SequenceNode)
                    {
                        throw new InvalidEventException();
                    }
                    break;

                case StateModelEventType.SequenceStepAdded:
                    if (modelEvent.Payload is not SequenceStep)
                    {
                        throw new InvalidEventException();
                    }
                    break;
            }
        }
    }
}
