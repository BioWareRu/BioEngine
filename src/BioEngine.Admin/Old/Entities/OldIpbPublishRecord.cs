using System;

#nullable disable

namespace BioEngine.Admin.Old.Entities
{
    public partial class OldIpbPublishRecord
    {
        public Guid Id { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateUpdated { get; set; }
        public Guid ContentId { get; set; }
        public string Type { get; set; }
        public int TopicId { get; set; }
        public int PostId { get; set; }
        public Guid[] SiteIds { get; set; }
    }
}
