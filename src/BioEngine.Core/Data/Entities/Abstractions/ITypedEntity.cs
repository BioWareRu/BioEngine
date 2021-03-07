using Sitko.Core.Repository;

namespace BioEngine.Core.Data.Entities.Abstractions
{
    public interface ITypedEntity : IEntity
    {
    }

    public interface ITypedEntity<T> : ITypedEntity where T : ITypedData, new()
    {
        T Data { get; set; }
    }

    public interface ITypedData
    {
    }
}
