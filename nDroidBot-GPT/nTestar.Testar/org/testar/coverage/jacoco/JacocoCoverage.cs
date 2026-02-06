using org.testar.coverage;
using org.testar.settings;

namespace org.testar.coverage.jacoco
{
    public class JacocoCoverage : CodeCoverage
    {
        public JacocoCoverage(Settings settings, string outputPath)
        {
            // TODO: Provide concrete JaCoCo-equivalent coverage implementation in .NET.
        }

        public void getSequenceCoverage()
        {
            throw new System.NotImplementedException();
        }

        public void getActionCoverage(string actionCount)
        {
            throw new System.NotImplementedException();
        }
    }
}
