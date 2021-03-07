using System;
using System.Collections.Generic;

namespace BioEngine.Core.Data.Entities.Abstractions
{
    public interface ITaggedContentEntity : IContentEntity
    {
        Guid[] TagIds { get; set; }
        List<Tag> Tags { get; set; }
    }
}
