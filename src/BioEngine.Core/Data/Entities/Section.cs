using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BioEngine.Core.Data.Entities.Abstractions;
using BioEngine.Core.Data.Entities.Blocks;
using Sitko.Core.Storage;

namespace BioEngine.Core.Data.Entities
{
    [Table("Sections")]
    public abstract class Section : BaseEntity, IContentEntity
    {
        public string Title { get; set; } = string.Empty;
        public abstract SectionType Type { get; set; }
        public virtual Guid? ParentId { get; set; }
        public bool IsPublished { get; set; }
        public DateTimeOffset? DatePublished { get; set; }
        public string Url { get; set; } = string.Empty;

        public List<ContentBlock> Blocks { get; set; } = new();
        public List<Post> Posts { get; set; } = new();
        public List<Site> Sites { get; set; } = new();
    }

    public abstract class Section<T> : Section where T : SectionData, new()
    {
        [Column(TypeName = "jsonb")] public virtual T Data { get; set; } = new();
    }

    public abstract record SectionData
    {
        public StorageItem? HeaderPicture { get; set; }
        public string? Hashtag { get; set; }
    }

    public enum SectionType
    {
        None,
        Developer,
        Game,
        Topic
    }
}
