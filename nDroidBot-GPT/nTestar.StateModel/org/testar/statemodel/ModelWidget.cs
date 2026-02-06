using System;
using System.Collections.Generic;

namespace org.testar.statemodel
{
    public class ModelWidget : TaggableEntity
    {
        private readonly string _id;
        private readonly List<ModelWidget> _children;
        private ModelWidget _parent;
        private ConcreteState _rootWidget;

        public ModelWidget(string id)
        {
            _id = id ?? throw new ArgumentNullException(nameof(id), "ModelWidget ID cannot be null");
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("ModelWidget ID cannot be empty or blank", nameof(id));
            }

            _children = new List<ModelWidget>();
        }

        public string GetId()
        {
            return _id;
        }

        public void AddChild(ModelWidget child)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child), "Child widget cannot be null");
            }

            _children.Add(child);
            child.SetParent(this);
        }

        public IReadOnlyList<ModelWidget> GetChildren()
        {
            return _children.AsReadOnly();
        }

        public ModelWidget GetParent()
        {
            return _parent;
        }

        public void SetParent(ModelWidget parent)
        {
            _parent = parent;
        }

        public ConcreteState GetRootWidget()
        {
            return _rootWidget;
        }

        public void SetRootWidget(ConcreteState rootWidget)
        {
            _rootWidget = rootWidget;
        }
    }
}
