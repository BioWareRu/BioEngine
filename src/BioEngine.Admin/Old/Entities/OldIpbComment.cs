using System;

#nullable disable

namespace BioEngine.Admin.Old.Entities
{
    public partial class OldIpbComment
    {
        public Guid Id { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateUpdated { get; set; }
        public Guid ContentId { get; set; }
        public string AuthorId { get; set; }
        public Guid[] SiteIds { get; set; }
        public int PostId { get; set; }
        public int TopicId { get; set; }
        public Guid? ReplyTo { get; set; }
        public string Text { get; set; }
        public string ContentType { get; set; }
    }
}
