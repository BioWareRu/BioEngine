using System;
using BioEngine.Admin.Shared;
using BioEngine.Core.Data.Entities;
using BioEngine.Core.Data.Repositories;

namespace BioEngine.Admin.Pages.Tags
{
    public class TagsList : BaseBioEngineList<Tag, Guid, TagsRepository>
    {
    }
}
