using System.Collections.Generic;

namespace org.testar.statemodel.actionselector.model
{
    public class SelectorTree
    {
        private readonly SelectorNode _rootNode;
        private List<SelectorNode> _leafNodes;

        public SelectorTree(SelectorNode rootNode)
        {
            _rootNode = rootNode;
            _leafNodes = new List<SelectorNode>();
            rootNode.SetTree(this);
        }

        internal void NotifyNodeAdded(SelectorNode node)
        {
            if (_leafNodes.Count == 0)
            {
                _leafNodes.Add(node);
                return;
            }

            int currentTreeDepth = _leafNodes[0].GetDepth();
            if (node.GetDepth() == currentTreeDepth)
            {
                _leafNodes.Add(node);
            }
            else if (node.GetDepth() > currentTreeDepth)
            {
                _leafNodes = new List<SelectorNode> { node };
            }
        }

        public IReadOnlyList<SelectorNode> GetLeafNodes()
        {
            return _leafNodes;
        }

        public int GetMaxTreeDepth()
        {
            return _leafNodes[0].GetDepth();
        }
    }
}
