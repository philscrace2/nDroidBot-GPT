using System;
using System.Collections.Generic;
using org.testar.monkey.alayer;

namespace org.testar
{
    public class PrioritizeNewActionsSelector : RandomActionSelector, IActionExecutor, IActionDerive
    {
        private ISet<Action>? previousActions;
        private readonly Dictionary<Action, int> executedActions = new Dictionary<Action, int>();

        public ISet<Action> getPrioritizedActions(ISet<Action> actions)
        {
            Console.WriteLine("---------------------------------------------------------");
            ISet<Action> prioritizedActions = new HashSet<Action>();

            if (previousActions == null)
            {
                Console.WriteLine("no previous actions -> all actions are new actions");
                prioritizedActions = actions;
            }
            else
            {
                Console.WriteLine("not the first round, get the new actions compared to previous state");
                prioritizedActions = ActionSelectionUtils.getSetOfNewActions(actions, previousActions);
            }

            if (prioritizedActions.Count > 0 && executedActions.Count > 0)
            {
                var executedActionsToCheck = new HashSet<Action>();
                foreach (var entry in executedActions)
                {
                    if (entry.Value > 0)
                    {
                        executedActionsToCheck.Add(entry.Key);
                    }
                }

                Console.WriteLine("there are new actions to choose from and there are executed actions, checking if they have been already executed");
                prioritizedActions = ActionSelectionUtils.getSetOfNewActions(prioritizedActions, executedActionsToCheck);
            }

            if (prioritizedActions.Count == 0)
            {
                Console.WriteLine("no new and unexecuted actions, checking if any unexecuted actions");
                prioritizedActions = ActionSelectionUtils.getSetOfNewActions(actions, new HashSet<Action>(executedActions.Keys));
            }

            if (prioritizedActions.Count == 0)
            {
                Console.WriteLine("no unexecuted actions, returning all actions");
                prioritizedActions = actions;

                Console.WriteLine("reset executed actions, size = " + executedActions.Count);
                int numberOfCleanedActions = 0;
                var actionList = new List<Action>(actions);
                foreach (var entry in executedActions)
                {
                    foreach (Action action in actionList)
                    {
                        if (ActionSelectionUtils.areSimilarActions(entry.Key, action))
                        {
                            executedActions[entry.Key] = 0;
                            numberOfCleanedActions++;
                            break;
                        }
                    }
                }
                Console.WriteLine("reseted actions = " + numberOfCleanedActions);
            }

            previousActions = actions;
            return prioritizedActions;
        }

        public void addExecutedAction(Action action)
        {
            foreach (var entry in executedActions)
            {
                if (ActionSelectionUtils.areSimilarActions(entry.Key, action))
                {
                    executedActions[entry.Key] = entry.Value + 1;
                    return;
                }
            }

            executedActions[action] = 1;
        }

        public int timesExecuted(Action action)
        {
            foreach (var entry in executedActions)
            {
                if (ActionSelectionUtils.areSimilarActions(entry.Key, action))
                {
                    return entry.Value;
                }
            }
            return 0;
        }

        public void executeAction(Action action)
        {
            addExecutedAction(action);
            Console.WriteLine("Executed action: " + action.get(Tags.Desc, "NoCurrentDescAvailable")
                              + " -- Times executed: " + timesExecuted(action));
        }

        public ISet<Action> deriveActions(ISet<Action> actions)
        {
            return getPrioritizedActions(actions);
        }
    }
}
