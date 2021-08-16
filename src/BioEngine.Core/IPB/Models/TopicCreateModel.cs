using System;

namespace BioEngine.Core.IPB.Models
{
    public class TopicCreateModel
    {
        public int? Forum { get; set; }
        public string Title { get; set; } = "";
        public int? Hidden { get; set; }
        public int? Pinned { get; set; }
        public string Post { get; set; } = "";
        public int Author { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
