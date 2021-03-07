using System;
using System.Collections.Generic;
using Sitko.Core.Repository;

namespace BioEngine.Core.Data.Entities.Abstractions
{
    public interface ISectionEntity : IEntity
    {
        List<Section> Sections { get; set; }
        List<Tag> Tags { get; set; }
    }
}
