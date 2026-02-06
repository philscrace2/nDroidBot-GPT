namespace org.testar.monkey.alayer
{
    public interface IWindowsAutomationProvider
    {
        StateBuilder CreateStateBuilder();
        HitTester CreateHitTester();
        Canvas CreateCanvas(Pen pen);
    }
}
