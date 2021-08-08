using System;

namespace BioEngine.Core.Data.Entities
{
    public class Developer : Section<DeveloperData>
    {
        public override SectionType Type { get; set; } = SectionType.Developer;
    }

    public record DeveloperData : SectionData
    {
        //public ValueCollection<Person> Persons { get; set; } = new();
    }

    public record Person
    {
        public string Name { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public DateTimeOffset DateStart { get; set; }
        public DateTimeOffset? DateEnd { get; set; }
    }
}
