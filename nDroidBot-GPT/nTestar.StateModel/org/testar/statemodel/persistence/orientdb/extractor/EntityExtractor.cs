using org.testar.statemodel.persistence.orientdb.entity;

namespace org.testar.statemodel.persistence.orientdb.extractor
{
    public interface EntityExtractor<T>
    {
        T Extract(DocumentEntity entity);
    }
}
