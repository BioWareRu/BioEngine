using System;
using System.ComponentModel.DataAnnotations.Schema;
using BioEngine.Core.Extensions;

#nullable disable

namespace BioEngine.Admin.Old.Entities
{
    public partial class OldStorageItem
    {
        public Guid Id { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateUpdated { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string PublicUri { get; set; }
        public string FilePath { get; set; }
        public OldStorageItemType Type { get; set; }
        [Column(TypeName = "jsonb")] public OldStorageItemPictureInfo? PictureInfo { get; set; }
        public string Path { get; set; }
        public string Hash { get; set; }
    }

    public enum OldStorageItemType
    {
        Picture = 1,
        Other = 2
    }

    public class OldStorageItemPictureInfo
    {
        public double VerticalResolution { get; set; }
        public double HorizontalResolution { get; set; }

        public OldStorageItemPictureThumbnail? LargeThumbnail { get; set; }
        public OldStorageItemPictureThumbnail? MediumThumbnail { get; set; }
        public OldStorageItemPictureThumbnail? SmallThumbnail { get; set; }
    }

    public class OldStorageItemPictureThumbnail
    {
        public Uri PublicUri { get; set; }
        public string FilePath { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public OldStorageItemPictureThumbnail()
        {
        }

        public OldStorageItemPictureThumbnail(Uri publicUri, string filePath, int width, int height)
        {
            PublicUri = publicUri;
            FilePath = filePath;
            Width = width;
            Height = height;
        }
    }
}
