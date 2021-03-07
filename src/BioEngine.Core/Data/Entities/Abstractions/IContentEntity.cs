using System.Collections.Generic;
using BioEngine.Core.Data.Entities.Blocks;

namespace BioEngine.Core.Data.Entities.Abstractions
{
    public interface IContentEntity : ISiteEntity, IRoutable, IBlocksItem, IPublishable
    {
        string Title { get; set; }
    }

    public interface IBlocksItem : IBioEntity
    {
        List<ContentBlock> Blocks { get; set; }
    }

    public enum ContentEntityViewMode
    {
        List,
        Entity
    }
}
