using System;
using Sitko.Core.Repository;

namespace BioEngine.Core.Data.Entities.Abstractions
{
    public interface IBioEntity : IEntity<Guid>
    {
        DateTimeOffset DateAdded { get; set; }
        DateTimeOffset DateUpdated { get; set; }
    }
}
