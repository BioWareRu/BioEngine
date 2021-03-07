using System;
using System.Linq;
using System.Text.Json.Serialization;
using FluentValidation;

namespace BioEngine.Core.Data.Entities.Blocks
{
    public record YoutubeBlock : ContentBlock<YoutubeBlockData>
    {
        public override string ToString()
        {
            return $"Youtube: {Data.YoutubeId}";
        }
    }

    public record YoutubeBlockData : ContentBlockData
    {
        public string YoutubeId { get; set; } = "";

        [JsonIgnore]
        public string? YoutubeLink
        {
            get
            {
                return string.IsNullOrEmpty(YoutubeId) ? null : $"https://www.youtube.com/watch?v={YoutubeId}";
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && (value.Contains("http://") || value.Contains("https://")))
                {
                    var uri = new Uri(value);

                    var queryParams = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

                    YoutubeId = queryParams.ContainsKey("v") ? queryParams["v"][0] : uri.Segments.Last();
                }
                else
                {
                    YoutubeId = string.Empty;
                }
            }
        }

        [JsonIgnore]
        public string? EmbedUrl =>
            string.IsNullOrEmpty(YoutubeId) ? null : $"https://www.youtube.com/embed/{YoutubeId}";
    }

    public class YoutubeBlockValidator : AbstractValidator<YoutubeBlock>
    {
        public YoutubeBlockValidator()
        {
            RuleFor(d => d.Data.YoutubeLink).NotEmpty().WithMessage("Укажите ссылку на видео");
        }
    }
}
