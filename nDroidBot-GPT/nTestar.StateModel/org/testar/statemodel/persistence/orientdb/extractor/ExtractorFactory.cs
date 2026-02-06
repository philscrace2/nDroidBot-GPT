using System;

namespace org.testar.statemodel.persistence.orientdb.extractor
{
    public static class ExtractorFactory
    {
        public const string ExtractorAbstractState = "ABSTRACT_STATE";
        public const string ExtractorAbstractAction = "ABSTRACT_ACTION";
        public const string ExtractorAbstractStateTransition = "ABSTRACT_STATE_TRANSITION";

        public static EntityExtractor<T> GetExtractor<T>(string name)
        {
            throw new NotImplementedException("TODO: Implement extractor selection when graph DB backend is ready.");
        }
    }
}
