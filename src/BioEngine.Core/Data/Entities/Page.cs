using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BioEngine.Core.Data.Entities.Abstractions;
using BioEngine.Core.Data.Entities.Blocks;

namespace BioEngine.Core.Data.Entities
{
    [Table("Pages")]
    public class Page : BaseEntity, IContentEntity
    {
        public List<Site> Sites { get; set; } = new();
        public string Url { get; set; }
        public List<ContentBlock> Blocks { get; set; } = new();
        public string Title { get; set; }
        public bool IsPublished { get; set; }
        public DateTimeOffset? DatePublished { get; set; }
    }
}
