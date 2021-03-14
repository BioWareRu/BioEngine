using System;
using System.ComponentModel.DataAnnotations.Schema;
using BioEngine.Core.Data.Entities.Abstractions;

#nullable disable

namespace BioEngine.Admin.Old.Entities
{
    public class OldContentBlock
    {
        public Guid Id { get; set; }
        public Guid ContentId { get; set; }
        public string Type { get; set; }
        public int Position { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateUpdated { get; set; }
        [Column(TypeName = "jsonb")] public string Data { get; set; }
    }

    public abstract class ContentBlockData : ITypedData
    {
    }

    public class OldYoutubeBlockData : ContentBlockData
    {
        public string YoutubeId { get; set; } = "";
    }

    public class OldTwitchBlockData : ContentBlockData
    {
        public string VideoId { get; set; }
        public string ChannelId { get; set; }
        public string CollectionId { get; set; }
    }

    public class OldTwitterBlockData : ContentBlockData
    {
        public string TweetId { get; set; } = "";
        public string TweetAuthor { get; set; } = "";
    }

    public class OldTextBlockData : ContentBlockData
    {
        public string Text { get; set; } = "";
    }

    public class OldQuoteBlockData : ContentBlockData
    {
        public string Text { get; set; } = string.Empty;
        public string? Author { get; set; }
        public string? Link { get; set; }
        public OldStorageItem? Picture { get; set; }
    }

    public class OldCutBlockData : ContentBlockData
    {
        public string ButtonText { get; set; } = "Читать дальше";
    }

    public class OldPictureBlockData : ContentBlockData
    {
        public OldStorageItem? Picture { get; set; }
        public string? Url { get; set; }
    }

    public class OldIframeBlockData : ContentBlockData
    {
        public string Src { get; set; } = "";
        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;
    }

    public class OldFileBlockData : ContentBlockData
    {
        public OldStorageItem File { get; set; } = new OldStorageItem();
    }

    public class OldGalleryBlockData : ContentBlockData
    {
        public OldStorageItem[] Pictures { get; set; } = new OldStorageItem[0];
    }
}
