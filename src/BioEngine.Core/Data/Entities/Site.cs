using System.Collections.Generic;

namespace BioEngine.Core.Data.Entities
{
    public class Site : BaseEntity
    {
        public string Url { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public List<Post> Posts { get; set; }
        public List<Page> Pages { get; set; }
        public List<Section> Sections { get; set; }
    }
}
