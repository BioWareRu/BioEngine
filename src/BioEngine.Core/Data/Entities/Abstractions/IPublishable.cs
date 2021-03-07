using System;

namespace BioEngine.Core.Data.Entities.Abstractions
{
    public interface IPublishable : IBioEntity
    {
        bool IsPublished { get; set; }
        DateTimeOffset? DatePublished { get; set; }
    }
}
