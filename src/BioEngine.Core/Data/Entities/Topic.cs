namespace BioEngine.Core.Data.Entities
{
    public class Topic : Section<TopicData>
    {
        public override SectionType Type { get; set; } = SectionType.Topic;
    }

    public record TopicData : SectionData
    {
    }
}
