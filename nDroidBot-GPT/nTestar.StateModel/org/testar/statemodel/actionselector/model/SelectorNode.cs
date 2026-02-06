using System.Collections.Generic;

namespace org.testar.statemodel.actionselector.model
{
    public class SelectorNode
    {
        private readonly AbstractState _abstractState;
        private readonly AbstractAction _abstractAction;
        private readonly List<SelectorNode> _children;
        private readonly SelectorNode _parent;
        private SelectorTree _tree;
        private readonly int _depth;

        public SelectorNode(AbstractState abstractState, AbstractAction abstractAction, int depth, SelectorNode parent)
        {
            _abstractState = abstractState;
            _abstractAction = abstractAction;
            _children = new List<SelectorNode>();
            _depth = depth;
            _parent = parent;
        }

        public void AddChild(SelectorNode node)
        {
            _children.Add(node);
        }

        public SelectorNode GetParent()
        {
            return _parent;
        }

        public AbstractState GetAbstractState()
        {
            return _abstractState;
        }

        public AbstractAction GetAbstractAction()
        {
            return _abstractAction;
        }

        public IReadOnlyList<SelectorNode> GetChildren()
        {
            return _children;
        }

        public bool IsRoot()
        {
            return _parent == null;
        }

        public bool IsLeaf()
        {
            return _children.Count == 0;
        }

        public void SetTree(SelectorTree tree)
        {
            _tree = tree;
            _tree.NotifyNodeAdded(this);
        }

        public SelectorTree GetTree()
        {
            return _tree;
        }

        public int GetDepth()
        {
            return _depth;
        }

        public LinkedList<SelectorNode> GetNodePath()
        {
            LinkedList<SelectorNode> nodeList = new LinkedList<SelectorNode>();
            if (_parent != null)
            {
                foreach (SelectorNode node in _parent.GetNodePath())
                {
                    nodeList.AddLast(node);
                }
            }
            nodeList.AddLast(this);
            return nodeList;
        }
    }
}
