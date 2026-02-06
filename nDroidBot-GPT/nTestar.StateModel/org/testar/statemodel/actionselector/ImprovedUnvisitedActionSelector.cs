using System;
using System.Collections.Generic;
using System.Linq;
using org.testar.statemodel.actionselector.model;
using org.testar.statemodel.exceptions;

namespace org.testar.statemodel.actionselector
{
    public class ImprovedUnvisitedActionSelector : ActionSelector
    {
        private LinkedList<AbstractAction> _executionPath;
        private const int MaxFlowAlterations = 1;
        private int _nrOfFlowAlterations;

        public ImprovedUnvisitedActionSelector()
        {
            _executionPath = new LinkedList<AbstractAction>();
            _nrOfFlowAlterations = 0;
        }

        public void NotifyNewSequence()
        {
            _executionPath = new LinkedList<AbstractAction>();
            Console.WriteLine("Reset State Model execution path due to new sequence starting");
        }

        public AbstractAction SelectAction(AbstractState currentState, AbstractStateModel abstractStateModel)
        {
            if (_nrOfFlowAlterations >= MaxFlowAlterations)
            {
                Console.WriteLine("Too many alterations in the flow. Throwing exception.");
                throw new ActionNotFoundException();
            }

            if (_executionPath.Count > 0)
            {
                AbstractAction nextInLine = _executionPath.First.Value;
                _executionPath.RemoveFirst();
                if (currentState.GetActionIds().Contains(nextInLine.GetActionId()))
                {
                    return nextInLine;
                }

                Console.WriteLine($"Action selection expected to be able to return action with id: {nextInLine.GetActionId()} , but the flow was altered");
                _nrOfFlowAlterations++;
                _executionPath = new LinkedList<AbstractAction>();
            }

            SelectorNode rootNode = new SelectorNode(currentState, null, 0, null);
            SelectorTree tree = new SelectorTree(rootNode);
            _executionPath = RetrieveUnvisitedActions(tree, abstractStateModel, new HashSet<string>());

            if (_executionPath.Count == 0)
            {
                throw new ActionNotFoundException();
            }

            string path = string.Join(", ", _executionPath.Select(action => action.GetActionId()));
            Console.WriteLine("New execution path: " + path);
            AbstractAction first = _executionPath.First.Value;
            _executionPath.RemoveFirst();
            return first;
        }

        private LinkedList<AbstractAction> RetrieveUnvisitedActions(SelectorTree tree, AbstractStateModel abstractStateModel, ISet<string> visitedStateIds)
        {
            List<LinkedList<SelectorNode>> nodePaths = tree.GetLeafNodes()
                .Where(node => node.GetAbstractState() != null && node.GetAbstractState().GetUnvisitedActions().Count > 0)
                .Select(node => node.GetNodePath())
                .ToList();

            if (nodePaths.Count > 0)
            {
                nodePaths.Sort((listA, listB) =>
                {
                    int sizeOfA = listA.Last.Value.GetAbstractState().GetUnvisitedActions().Count;
                    int sizeOfB = listB.Last.Value.GetAbstractState().GetUnvisitedActions().Count;
                    return sizeOfB - sizeOfA;
                });

                LinkedList<SelectorNode> nodePath = nodePaths[0];
                IEnumerable<AbstractAction> actionStream = nodePath
                    .Select(node => node.GetAbstractAction())
                    .Where(action => action != null);

                long graphTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                Random rnd = new Random((int)(graphTime & 0x7FFFFFFF));
                List<AbstractAction> unvisitedActions = new List<AbstractAction>(nodePath.Last.Value.GetAbstractState().GetUnvisitedActions());
                AbstractAction unvisitedAction = unvisitedActions[rnd.Next(unvisitedActions.Count)];

                LinkedList<AbstractAction> result = new LinkedList<AbstractAction>();
                foreach (AbstractAction action in actionStream)
                {
                    result.AddLast(action);
                }
                result.AddLast(unvisitedAction);
                return result;
            }

            foreach (SelectorNode node in tree.GetLeafNodes())
            {
                if (node.GetAbstractState() != null)
                {
                    visitedStateIds.Add(node.GetAbstractState().GetStateId());
                }
            }

            int currentTreeDepth = tree.GetMaxTreeDepth();

            foreach (SelectorNode node in tree.GetLeafNodes())
            {
                AbstractState abstractState = node.GetAbstractState();
                if (abstractState == null)
                {
                    continue;
                }

                foreach (AbstractStateTransition transition in abstractStateModel.GetOutgoingTransitionsForState(abstractState.GetStateId()))
                {
                    if (!visitedStateIds.Contains(transition.GetTargetStateId()))
                    {
                        visitedStateIds.Add(transition.GetTargetStateId());
                        SelectorNode childNode = new SelectorNode(transition.GetTargetState(), transition.GetAction(), node.GetDepth() + 1, node);
                        node.AddChild(childNode);
                        childNode.SetTree(node.GetTree());
                    }
                }
            }

            if (tree.GetMaxTreeDepth() == currentTreeDepth)
            {
                return new LinkedList<AbstractAction>();
            }

            return RetrieveUnvisitedActions(tree, abstractStateModel, visitedStateIds);
        }
    }
}
