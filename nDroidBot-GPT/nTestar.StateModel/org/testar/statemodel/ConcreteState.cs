using System;
using org.testar.statemodel.persistence;

namespace org.testar.statemodel
{
    public class ConcreteState : ModelWidget, Persistable
    {
        private readonly AbstractState _abstractState;
        private byte[] _screenshot;

        public ConcreteState(string id, AbstractState abstractState)
            : base(id ?? throw new ArgumentNullException(nameof(id), "ConcreteState ID cannot be null"))
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("ConcreteState ID cannot be empty or blank", nameof(id));
            }

            SetRootWidget(this);
            _abstractState = abstractState ?? throw new ArgumentNullException(nameof(abstractState), "AbstractState cannot be null");
        }

        public byte[] GetScreenshot()
        {
            return _screenshot != null ? (byte[])_screenshot.Clone() : Array.Empty<byte>();
        }

        public void SetScreenshot(byte[] screenshot)
        {
            _screenshot = screenshot != null ? (byte[])screenshot.Clone() : null;
        }

        public AbstractState GetAbstractState()
        {
            return _abstractState;
        }

        public bool CanBeDelayed()
        {
            return true;
        }
    }
}
