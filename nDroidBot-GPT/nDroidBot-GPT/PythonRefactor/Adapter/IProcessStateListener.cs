public partial class ProcessMonitor
{
    public interface IProcessStateListener
    {
        void OnStateUpdated(Dictionary<string, string> pid2name);
    }
}

