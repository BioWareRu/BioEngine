using System;
using System.Linq;
using System.Text.Json.Serialization;
using FluentValidation;

namespace BioEngine.Core.Data.Entities.Blocks
{
    public record TwitchBlock : ContentBlock<TwitchBlockData>
    {
        public override string ToString()
        {
            return $"Twitch: {Data.VideoId}{Data.ChannelId}{Data.CollectionId}";
        }
    }

    public record TwitchBlockData : ContentBlockData
    {
        public string? VideoId { get; set; }
        public string? ChannelId { get; set; }
        public string? CollectionId { get; set; }

        [JsonIgnore]
        public string? TwitchLink
        {
            get
            {
                var url = "https://player.twitch.tv/";
                if (!string.IsNullOrEmpty(VideoId))
                {
                    url += $"?video={VideoId}";
                }
                else if (!string.IsNullOrEmpty(CollectionId))
                {
                    url += $"?collection={CollectionId}";
                }
                else if (!string.IsNullOrEmpty(ChannelId))
                {
                    url += $"?channel={ChannelId}";
                }
                else
                {
                    return null;
                }

                return url;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
                    {
                        if (uri.Host == "player.twitch.tv")
                        {
                            var queryParams = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
                            if (queryParams.ContainsKey("video"))
                            {
                                VideoId = queryParams["video"][0];
                                CollectionId = null;
                                ChannelId = null;
                            }
                            else if (queryParams.ContainsKey("channel"))
                            {
                                VideoId = null;
                                CollectionId = null;
                                ChannelId = queryParams["channel"][0];
                            }
                            else if (queryParams.ContainsKey("collection"))
                            {
                                VideoId = null;
                                CollectionId = queryParams["collection"][0];
                                ChannelId = null;
                            }
                            else
                            {
                                VideoId = null;
                                CollectionId = null;
                                ChannelId = null;
                            }
                        }
                        else if ((uri.Host == "twitch.tv" || uri.Host == "www.twitch.tv") && uri.AbsolutePath.StartsWith("/videos"))
                        {
                            VideoId = uri.AbsolutePath.Split('/').Last();
                            CollectionId = null;
                            ChannelId = null;
                        }
                    }
                    else
                    {
                        VideoId = null;
                        CollectionId = null;
                        ChannelId = null;
                    }
                }
                else
                {
                    VideoId = null;
                    CollectionId = null;
                    ChannelId = null;
                }
            }
        }
    }

    public class TwitchBlockValidator : AbstractValidator<TwitchBlock>
    {
        public TwitchBlockValidator()
        {
            RuleFor(d => d.Data.TwitchLink).NotEmpty().WithMessage("Укажите ссылку на видео")
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)).WithMessage("Значение должно быть ссылкой");
        }
    }
}
