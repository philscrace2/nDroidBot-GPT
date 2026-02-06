using System;
using org.testar.statemodel.persistence.graphdb;

namespace org.testar.statemodel.persistence.orientdb.hydrator
{
    public static class HydratorFactory
    {
        public const string HydratorAbstractState = "ABSTRACT_STATE";
        public const string HydratorAbstractAction = "ABSTRACT_ACTION";
        public const string HydratorAbstractStateModel = "ABSTRACT_STATE_MODEL";
        public const string HydratorConcreteState = "CONCRETE_STATE";
        public const string HydratorConcreteAction = "CONCRETE_ACTION";
        public const string HydratorSequence = "SEQUENCE";
        public const string HydratorSequenceNode = "SEQUENCE_NODE";
        public const string HydratorSequenceStep = "SEQUENCE_STEP";
        public const string HydratorWidget = "WIDGET";
        public const string HydratorWidgetRelation = "WIDGET_RELATION";
        public const string HydratorIsAbstractedBy = "IS_ABSTRACTED_BY";
        public const string HydratorAccessed = "ACCESSED";
        public const string HydratorFirstNode = "FIRST_NODE";
        public const string HydratorBlackHole = "BLACKHOLE";

        public static EntityHydrator GetHydrator(string name)
        {
            throw new NotImplementedException("TODO: Implement hydrator selection when graph DB backend is ready.");
        }
    }
}
