using System;
using System.Linq;
using System.Text.Json.Serialization;
using FluentValidation;

namespace BioEngine.Core.Data.Entities.Blocks
{
    public record TwitterBlock : ContentBlock<TwitterBlockData>
    {
        public override string ToString()
        {
            return $"Twitter: {Data.TweetId} by {Data.TweetAuthor}";
        }
    }

    public record TwitterBlockData : ContentBlockData{
        public string TweetId { get; set; } = "";
        public string TweetAuthor { get; set; } = "";

        [JsonIgnore]
        public string? TweetLink
        {
            get
            {
                return string.IsNullOrEmpty(TweetId) ? null : $"https://twitter.com/{TweetAuthor}/status/{TweetId}";
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var uri = new Uri(value);

                    if (uri.Segments.Length == 4)
                    {
                        var author = uri.Segments[1].Replace("/", "");
                        var tweetId = uri.Segments.Last();
                        if (TweetAuthor != author || TweetId != tweetId)
                        {
                            TweetAuthor = author;
                            TweetId = tweetId;
                        }
                    }
                }
                else
                {
                    TweetId = string.Empty;
                    TweetAuthor = string.Empty;
                }
            }
        }
    }

    public class TwitterBlockValidator : AbstractValidator<TwitterBlock>
    {
        public TwitterBlockValidator()
        {
            RuleFor(d => d.Data.TweetLink).NotEmpty().WithMessage("Укажите ссылку на твит")
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)).WithMessage("Значение должно быть ссылкой");
        }
    }
}
