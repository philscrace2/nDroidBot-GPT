using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using org.testar.monkey;
using org.testar.monkey.alayer;

namespace org.testar.serialisation
{
    public sealed class TestSerialiser
    {
        private static readonly ConcurrentQueue<TaggableBase> TestSavingQueue = new();
        private static readonly AutoResetEvent QueueSignal = new(false);
        private static Stream? testStream;
        private static Thread? worker;
        private static volatile bool alive;
        private static int fragmentTimes;

        public static void Start(Stream outputStream)
        {
            Assert.isTrue(!alive);
            while (TestSavingQueue.TryDequeue(out _)) { }

            testStream = outputStream;
            fragmentTimes = 0;
            alive = true;
            worker = new Thread(Run) { IsBackground = true };
            worker.Start();
        }

        public static void Finish()
        {
            alive = false;
            QueueSignal.Set();
        }

        public static bool IsSavingQueueEmpty()
        {
            return TestSavingQueue.IsEmpty;
        }

        public static void Write(TaggableBase fragment)
        {
            if (!alive)
            {
                return;
            }

            TestSavingQueue.Enqueue(fragment);
            QueueSignal.Set();
        }

        public static void Exit()
        {
            if (worker == null)
            {
                return;
            }

            Finish();
            worker.Join();
            worker = null;
            testStream = null;
        }

        public static int QueueLength()
        {
            return TestSavingQueue.Count;
        }

        private static void Run()
        {
            while (alive || !TestSavingQueue.IsEmpty)
            {
                if (!TestSavingQueue.TryDequeue(out TaggableBase? fragment))
                {
                    QueueSignal.WaitOne(1000);
                    continue;
                }

                WriteFragment(fragment);
            }

            testStream?.Flush();
            testStream?.Dispose();
        }

        private static void WriteFragment(TaggableBase fragment)
        {
            Assert.notNull(fragment);
            Assert.notNull(testStream);

            using var writer = new StreamWriter(testStream!, leaveOpen: true);
            writer.WriteLine(fragment.ToString());
            fragmentTimes++;
            if (fragmentTimes >= 16)
            {
                fragmentTimes = 0;
                writer.Flush();
            }
        }
    }
}
