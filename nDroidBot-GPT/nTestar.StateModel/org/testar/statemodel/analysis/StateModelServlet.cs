using System.Collections.Generic;
using org.testar.statemodel.analysis.representation;

namespace org.testar.statemodel.analysis
{
    public static class StateModelServlet
    {
        public static List<AbstractStateModel> HandleModelsRequest(AnalysisManager analysisManager)
        {
            return analysisManager.FetchModels();
        }

        public static List<ActionViz> HandleSequenceRequest(AnalysisManager analysisManager, string sequenceId)
        {
            return analysisManager.FetchTestSequence(sequenceId);
        }
    }
}
