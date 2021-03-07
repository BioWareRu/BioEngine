using Sitko.Core.Repository;

namespace BioEngine.Core.Data.Entities.Abstractions
{
    public interface IRoutable : IEntity
    {
        string Url { get; set; }
    }
}
