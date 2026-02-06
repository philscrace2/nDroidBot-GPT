namespace org.testar.coverage
{
    public interface ICodeCoverageTool
    {
        void getSequenceCoverage();
        void getActionCoverage(string actionCount);
    }
}
