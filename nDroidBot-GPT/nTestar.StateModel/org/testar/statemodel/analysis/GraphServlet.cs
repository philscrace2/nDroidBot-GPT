namespace org.testar.statemodel.analysis
{
    public static class GraphServlet
    {
        public static string HandleGraphRequest(AnalysisManager analysisManager, string modelIdentifier, bool abstractLayerRequired, bool concreteLayerRequired, bool sequenceLayerRequired, bool showCompoundGraph)
        {
            return analysisManager.FetchGraphForModel(modelIdentifier, abstractLayerRequired, concreteLayerRequired, sequenceLayerRequired, showCompoundGraph);
        }

        public static string HandleWidgetTreeRequest(AnalysisManager analysisManager, string concreteStateIdentifier)
        {
            return analysisManager.FetchWidgetTree(concreteStateIdentifier);
        }
    }
}
