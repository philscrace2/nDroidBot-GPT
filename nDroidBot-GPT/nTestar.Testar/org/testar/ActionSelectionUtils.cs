using System.Collections.Generic;
using org.testar.monkey.alayer;
using State = org.testar.monkey.alayer.State;
using Action = org.testar.monkey.alayer.Action;

namespace org.testar
{
    public static class ActionSelectionUtils
    {
        public static ISet<Action> getSetOfNewActions(ISet<Action> currentActions, ISet<Action> previousActions)
        {
            ISet<Action> newActions = new HashSet<Action>();
            foreach (Action currentAction in currentActions)
            {
                bool matchingActionFound = false;
                foreach (Action previousAction in previousActions)
                {
                    if (areSimilarActions(previousAction, currentAction))
                    {
                        matchingActionFound = true;
                        break;
                    }
                }

                if (!matchingActionFound)
                {
                    newActions.Add(currentAction);
                }
            }

            return newActions;
        }

        public static bool areSimilarActions(Action action1, Action action2)
        {
            if (string.Equals(
                    action1.get(Tags.Desc, "NoCurrentDescAvailable"),
                    action2.get(Tags.Desc, "NoPreviousDescAvailable"),
                    System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (action1.get(Tags.Role, null) != null &&
                action2.get(Tags.Role, null) != null &&
                string.Equals(action1.get(Tags.Role, null)?.ToString(), "ClickTypeInto", System.StringComparison.OrdinalIgnoreCase) &&
                string.Equals(action2.get(Tags.Role, null)?.ToString(), "ClickTypeInto", System.StringComparison.OrdinalIgnoreCase))
            {
                string currentTargetDesc = action1.get(Tags.Desc, "currentDescNotAvailable");
                currentTargetDesc = currentTargetDesc.Substring(currentTargetDesc.IndexOf("into", System.StringComparison.Ordinal));
                string previousTargetDesc = action2.get(Tags.Desc, "previousDescNotAvailable");
                previousTargetDesc = previousTargetDesc.Substring(previousTargetDesc.IndexOf("into", System.StringComparison.Ordinal));
                if (string.Equals(currentTargetDesc, previousTargetDesc, System.StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
