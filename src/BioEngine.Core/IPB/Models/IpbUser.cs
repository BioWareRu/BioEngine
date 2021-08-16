using System;

namespace BioEngine.Core.IPB.Models
{
    public class IpbUser
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string PhotoUrl { get; set; } = "";
        public string ProfileUrl { get; set; } = "";
        public Group PrimaryGroup { get; set; } = new();
        public Group[] SecondaryGroups { get; set; } = Array.Empty<Group>();
    }
}