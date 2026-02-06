using System;

namespace org.testar.statemodel.exceptions
{
    public class StateModelException : Exception
    {
        public StateModelException()
        {
        }

        public StateModelException(string message)
            : base(message)
        {
        }

        public StateModelException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
