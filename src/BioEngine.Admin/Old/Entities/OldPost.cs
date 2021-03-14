using System;

#nullable disable

namespace BioEngine.Admin.Old.Entities
{
    public partial class OldPost
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string AuthorId { get; set; }
        public Guid[] SiteIds { get; set; }
        public Guid[] SectionIds { get; set; }
        public Guid[] TagIds { get; set; }
        public bool IsPublished { get; set; }
        public DateTimeOffset DateAdded { get; set; }
        public DateTimeOffset DateUpdated { get; set; }
        public DateTimeOffset? DatePublished { get; set; }
    }
}
