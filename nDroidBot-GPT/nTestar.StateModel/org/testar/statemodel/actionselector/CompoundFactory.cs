using System.Collections.Generic;
using org.testar.monkey.alayer;

namespace org.testar.statemodel.actionselector
{
    public static class CompoundFactory
    {
        public static CompoundActionSelector GetCompoundActionSelector(TaggableBase configTags)
        {
            List<ActionSelector> selectors = new List<ActionSelector>();
            if (configTags.get(StateModelTags.ActionSelectionAlgorithm) == "unvisited")
            {
                selectors.Add(new ImprovedUnvisitedActionSelector());
            }
            selectors.Add(new RandomActionSelector());
            return new CompoundActionSelector(selectors);
        }
    }
}
