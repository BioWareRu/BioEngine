using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using BioEngine.Core.Data.Entities.Abstractions;
using BioEngine.Core.Data.Entities.Blocks;
using BioEngine.Core.Users;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BioEngine.Core.Data.Entities
{
    [Table("Posts")]
    public class Post : BaseEntity, IContentItem
    {
        public string AuthorId { get; set; }
        [NotMapped] public User Author { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public bool IsPublished { get; set; }
        public DateTimeOffset? DatePublished { get; set; }

        public List<Section> Sections { get; set; } = new();
        public List<Site> Sites { get; set; } = new();
        public List<Tag> Tags { get; set; } = new();

        public List<ContentBlock> Blocks { get; set; } = new();
    }
}
