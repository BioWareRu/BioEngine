using System.ComponentModel.DataAnnotations.Schema;

namespace BioEngine.Core.Data.Entities
{
    public class Game : Section<GameData>
    {
        public override SectionType Type { get; set; } = SectionType.Game;
    }

    public record GameData : SectionData
    {
        //public Platform[] Platforms { get; set; } = new Platform[0];
    }

    public enum Platform
    {
        PC,
        Xbox,
        Xbox360,
        XboxOne,
        PSOne,
        PSTwo,
        PSThree,
        PSFour,
        Android,

        // ReSharper disable once InconsistentNaming
        iOS,
        MacOS,
        Linux
    }
}
