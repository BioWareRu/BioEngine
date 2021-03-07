using System;

namespace BioEngine.Core.Data.Entities
{
    public abstract class BaseEntity : Sitko.Core.Repository.Entity<Guid>
    {
        public virtual DateTimeOffset DateAdded { get; set; } = DateTimeOffset.UtcNow;
        public virtual DateTimeOffset DateUpdated { get; set; } = DateTimeOffset.UtcNow;
    }
}
