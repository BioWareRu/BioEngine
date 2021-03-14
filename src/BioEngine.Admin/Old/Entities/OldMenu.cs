using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace BioEngine.Admin.Old.Entities
{
    public partial class OldMenu
    {
        public Guid Id { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateUpdated { get; set; }
        public string Title { get; set; }
        [Column(TypeName = "jsonb")] public List<OldMenuItem> Items { get; set; } = new List<OldMenuItem>();
        public Guid[] SiteIds { get; set; }
    }

    public class OldMenuItem
    {
        public string Label { get; set; } = "";
        public string Url { get; set; } = "";
        public List<OldMenuItem> Items { get; set; } = new List<OldMenuItem>();
    }
}
