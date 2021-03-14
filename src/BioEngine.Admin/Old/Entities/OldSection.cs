using System;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace BioEngine.Admin.Old.Entities
{
    public partial class OldSection
    {
        public Guid Id { get; set; }
        public DateTimeOffset DateAdded { get; set; }
        public DateTimeOffset DateUpdated { get; set; }
        public bool IsPublished { get; set; }
        public DateTimeOffset? DatePublished { get; set; }
        public Guid[] SiteIds { get; set; }
        public string Type { get; set; }
        public Guid? ParentId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }

        [Column(TypeName = "jsonb")] public BrcSectionData Data { get; set; }
    }

    public class BrcSectionData
    {
        public OldStorageItem? HeaderPicture { get; set; }
        public string? Hashtag { get; set; }
    }
}
