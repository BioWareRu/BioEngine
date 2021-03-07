using System.Collections.Generic;

namespace BioEngine.Core.Data.Entities.Abstractions
{
    public interface ISiteEntity : IBioEntity
    {
        List<Site> Sites { get; set; }
    }
}
