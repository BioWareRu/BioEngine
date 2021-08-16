using System.Collections.Generic;
using Newtonsoft.Json;

namespace BioEngine.Core.IPB.Models
{
    public class Forum
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int Topics { get; set; }
        public int Position { get; set; }
        public string Url { get; set; } = "";
        [JsonProperty("parent_id")] public int? ParentId { get; set; }
    }
}
