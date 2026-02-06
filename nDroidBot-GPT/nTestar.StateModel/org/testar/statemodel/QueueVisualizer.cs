using System;

namespace org.testar.statemodel
{
    public class QueueVisualizer
    {
        private readonly string _title;

        public QueueVisualizer(string title)
        {
            _title = title;
            Console.WriteLine(title);
        }

        public void UpdateMessage(string message)
        {
            Console.WriteLine(message);
        }

        public void Stop()
        {
        }
    }
}
