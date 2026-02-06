using System.Collections.Generic;
using org.testar.statemodel.event;

namespace org.testar.statemodel
{
    public abstract class AbstractEntity : TaggableEntity
    {
        private readonly string _id;
        private string _modelIdentifier;
        private readonly HashSet<StateModelEventListener> _eventListeners;

        protected AbstractEntity(string id)
        {
            _id = id;
            _eventListeners = new HashSet<StateModelEventListener>();
        }

        public string GetId()
        {
            return _id;
        }

        public string GetModelIdentifier()
        {
            return _modelIdentifier;
        }

        public void SetModelIdentifier(string modelIdentifier)
        {
            _modelIdentifier = modelIdentifier;
        }

        public void AddEventListener(StateModelEventListener eventListener)
        {
            _eventListeners.Add(eventListener);
        }

        protected void EmitEvent(StateModelEvent modelEvent)
        {
            foreach (StateModelEventListener listener in _eventListeners)
            {
                listener.EventReceived(modelEvent);
            }
        }
    }
}
