using System;
using Sitko.Core.Storage;

namespace BioEngine.Core.Extensions
{
    public static class StorageExtensions
    {
        public static bool IsImage(this StorageItem storageItem)
        {
            var metadata = storageItem.GetMetadata<StorageItemMetadata>();
            return metadata != null && metadata.Type == StorageItemType.Image;
        }
    }

    public class StorageItemMetadata
    {
        public StorageItemType Type { get; set; } = StorageItemType.File;
        public StorageItemImageMetadata? ImageMetadata { get; set; }
    }

    public class StorageItemImageMetadata
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public enum StorageItemType
    {
        File,
        Image
    }
}
