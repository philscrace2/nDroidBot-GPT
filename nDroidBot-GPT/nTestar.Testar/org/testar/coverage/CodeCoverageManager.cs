using System.Collections.Generic;
using org.testar.settings;

namespace org.testar.coverage
{
    public class CodeCoverageManager : CodeCoverage
    {
        private readonly List<ICodeCoverageTool> coverageTools = new List<ICodeCoverageTool>();

        public CodeCoverageManager(Settings settings, IEnumerable<ICodeCoverageTool>? tools = null)
        {
            if (tools != null)
            {
                coverageTools.AddRange(tools);
            }
        }

        public void RegisterTool(ICodeCoverageTool tool)
        {
            coverageTools.Add(tool);
        }

        public void getSequenceCoverage()
        {
            foreach (var tool in coverageTools)
            {
                tool.getSequenceCoverage();
            }
        }

        public void getActionCoverage(string actionCount)
        {
            foreach (var tool in coverageTools)
            {
                tool.getActionCoverage(actionCount);
            }
        }
    }
}
