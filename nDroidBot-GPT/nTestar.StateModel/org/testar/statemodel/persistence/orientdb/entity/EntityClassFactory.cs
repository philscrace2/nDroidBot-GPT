using System.Collections.Generic;

namespace org.testar.statemodel.persistence.orientdb.entity
{
    public static class EntityClassFactory
    {
        public enum EntityClassName
        {
            AbstractAction,
            AbstractState,
            AbstractStateModel,
            Widget,
            ConcreteState,
            ConcreteAction,
            IsParentOf,
            IsChildOf,
            IsAbstractedBy,
            BlackHole,
            UnvisitedAbstractAction,
            TestSequence,
            SequenceNode,
            SequenceStep,
            Accessed,
            FirstNode
        }

        public static EntityClass CreateEntityClass(EntityClassName className)
        {
            return new EntityClass(className.ToString());
        }
    }
}
