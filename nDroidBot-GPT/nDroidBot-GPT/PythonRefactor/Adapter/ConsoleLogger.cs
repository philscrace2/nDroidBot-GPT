public class ConsoleLogger : ILogger
{
    public void Debug(string message) => Console.WriteLine("[DEBUG] " + message);
    public void Info(string message) => Console.WriteLine("[INFO] " + message);
    public void Warn(string message) => Console.WriteLine("[WARN] " + message);
}

