using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using org.testar.monkey;

namespace org.testar.serialisation
{
    public sealed class LogSerialiser
    {
        public enum LogLevel
        {
            Critical = 0,
            Info = 1,
            Debug = 2
        }

        private sealed class LogRecord
        {
            public LogRecord(string message, LogLevel level)
            {
                Message = message;
                Level = level;
            }

            public string Message { get; }
            public LogLevel Level { get; }
        }

        private static readonly ConcurrentQueue<LogRecord> LogQueue = new();
        private static readonly AutoResetEvent LogSignal = new(false);
        private static TextWriter? log;
        private static int logLevel;
        private static int logTimes;
        private static volatile bool alive;
        private static Thread? worker;

        public static void Start(TextWriter logStream, int level)
        {
            Assert.isTrue(!alive);
            while (LogQueue.TryDequeue(out _)) { }

            log = logStream;
            logLevel = level;
            logTimes = 0;
            alive = true;
            worker = new Thread(Run) { IsBackground = true };
            worker.Start();
        }

        public static void Finish()
        {
            alive = false;
            LogSignal.Set();
        }

        public static void Log(string message)
        {
            if (alive)
            {
                Log(message, LogLevel.Info);
            }
        }

        public static void Log(string message, LogLevel level)
        {
            if (!alive || level > (LogLevel)logLevel)
            {
                return;
            }

            LogQueue.Enqueue(new LogRecord(message, level));
            LogSignal.Set();
        }

        public static void Flush()
        {
            log?.Flush();
        }

        public static TextWriter? GetLogStream()
        {
            return log;
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
            log = null;
        }

        public static int QueueLength()
        {
            return LogQueue.Count;
        }

        private static void Run()
        {
            while (alive || !LogQueue.IsEmpty)
            {
                if (!LogQueue.TryDequeue(out LogRecord? record))
                {
                    LogSignal.WaitOne(1000);
                    continue;
                }

                LogMessage(record.Message);
            }

            log?.Flush();
            log?.Dispose();
        }

        private static void LogMessage(string message)
        {
            Assert.notNull(log);
            log!.Write(message);
            logTimes++;
            if (logTimes >= 100)
            {
                logTimes = 0;
                log.Flush();
            }
        }
    }
}
