using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using org.testar.monkey;
using org.testar.monkey.alayer;

namespace org.testar.serialisation
{
    public sealed class ScreenshotSerialiser
    {
        public const string SCRSHOTS = "scrshots";
        private static readonly ConcurrentQueue<ScrshotRecord> ScrshotSavingQueue = new();
        private static readonly AutoResetEvent QueueSignal = new(false);
        private static string? testSequenceFolder;
        private static string? scrshotOutputFolder;
        private static Thread? worker;
        private static volatile bool alive;

        private sealed class ScrshotRecord
        {
            public ScrshotRecord(string path, AWTCanvas shot)
            {
                Path = path;
                Shot = shot;
            }

            public string Path { get; }
            public AWTCanvas Shot { get; }
        }

        public static void Start(string outputFolder, string sequenceFolder)
        {
            Assert.isTrue(!alive);
            while (ScrshotSavingQueue.TryDequeue(out _)) { }

            alive = true;
            scrshotOutputFolder = outputFolder;
            testSequenceFolder = sequenceFolder;
            Directory.CreateDirectory(System.IO.Path.Combine(outputFolder, sequenceFolder));
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
            return ScrshotSavingQueue.IsEmpty;
        }

        public static string SaveStateshot(string stateId, AWTCanvas stateshot)
        {
            string path = System.IO.Path.Combine(scrshotOutputFolder ?? ".", testSequenceFolder ?? ".", stateId + ".png");
            if (!File.Exists(path))
            {
                SaveThis(path, stateshot);
            }

            return path;
        }

        public static string SaveActionshot(string stateId, string actionId, AWTCanvas actionshot)
        {
            string path = System.IO.Path.Combine(scrshotOutputFolder ?? ".", testSequenceFolder ?? ".", stateId + "_" + actionId + ".png");
            if (!File.Exists(path))
            {
                SaveThis(path, actionshot);
            }

            return path;
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
            testSequenceFolder = null;
            scrshotOutputFolder = null;
        }

        public static int QueueLength()
        {
            return ScrshotSavingQueue.Count;
        }

        private static void SaveThis(string path, AWTCanvas shot)
        {
            if (!alive)
            {
                return;
            }

            ScrshotSavingQueue.Enqueue(new ScrshotRecord(path, shot));
            QueueSignal.Set();
        }

        private static void Run()
        {
            while (alive || !ScrshotSavingQueue.IsEmpty)
            {
                if (!ScrshotSavingQueue.TryDequeue(out ScrshotRecord? record))
                {
                    QueueSignal.WaitOne(300);
                    continue;
                }

                try
                {
                    string tempPath = record.Path + ".part";
                    record.Shot.SaveAsPng(tempPath);
                    File.Move(tempPath, record.Path, true);
                }
                catch (IOException)
                {
                    LogSerialiser.Log($"I/O exception saving screenshot <{record.Path}>\\n", LogSerialiser.LogLevel.Critical);
                }
            }
        }
    }
}
