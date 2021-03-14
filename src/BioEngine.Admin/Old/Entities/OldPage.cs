using System;

#nullable disable

namespace BioEngine.Admin.Old.Entities
{
    public partial class OldPage
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public Guid[] SiteIds { get; set; }
        public bool IsPublished { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateUpdated { get; set; }
        public DateTime? DatePublished { get; set; }
    }
}
