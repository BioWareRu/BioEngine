using System;

#nullable disable

namespace BioEngine.Admin.Old.Entities
{
    public partial class OldSite
    {
        public Guid Id { get; set; }
        public DateTimeOffset DateAdded { get; set; }
        public DateTimeOffset DateUpdated { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }
}
