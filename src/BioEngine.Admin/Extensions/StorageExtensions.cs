using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AntDesign;
using BioEngine.Admin.Shared.Storage;
using BioEngine.Core;
using BioEngine.Core.Extensions;
using Sitko.Core.Storage;
using Image = SixLabors.ImageSharp.Image;

namespace BioEngine.Admin.Extensions
{
    public static class StorageUploadExtensions
    {
        private static readonly string[] _imageExtensions = {".jpg", ".jpeg", ".png"};

        public static Task<StorageItem> ProcessAndUploadFileAsync(this IStorage<BRCStorageConfig> storage, Stream file,
            string fileName,
            string path)
        {
            var metaData = new StorageItemMetadata();
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (_imageExtensions.Contains(extension))
            {
                file.Position = 0;
                metaData.Type = StorageItemType.Image;
                using Image image = Image.Load(file);
                metaData.ImageMetadata = new StorageItemImageMetadata {Height = image.Height, Width = image.Width};
                file.Position = 0;
            }

            return storage.SaveAsync(file, fileName, path, metaData);
        }

        public static (string path, string name)[] GetPathParts(string path)
        {
            var parts = new List<(string path, string name)> {("/", "Начало")};
            if (!string.IsNullOrEmpty(path) && path != "/")
            {
                string[] directories = path.Split(Path.DirectorySeparatorChar);

                string previousEntry = string.Empty;
                foreach (string dir in directories)
                {
                    string newEntry = previousEntry + Path.DirectorySeparatorChar + dir;
                    if (!string.IsNullOrEmpty(newEntry))
                    {
                        if (!newEntry.Equals(Convert.ToString(Path.DirectorySeparatorChar),
                            StringComparison.OrdinalIgnoreCase))
                        {
                            parts.Add((newEntry.Trim(Path.DirectorySeparatorChar),
                                dir.Replace("/", "")));
                            previousEntry = newEntry;
                        }
                    }
                }
            }

            return parts.ToArray();
        }

        public static async Task SelectStorageItemsAsync(this ModalService modalService, Action<StorageItem[]> callback,
            string prefix = "storage", bool isMultiple = false)
        {
            var config = new StorageExplorerDialogOptions(prefix, isMultiple);
            await modalService.CreateModalAsync<StorageExplorerDialog, StorageExplorerDialogOptions>(
                new ModalOptions
                {
                    Footer = null,
                    Title = isMultiple ? "Выбор файлов" : "Выбор файла",
                    Width = "60%",
                    AfterClose =
                        () =>
                        {
                            callback(config.Items);
                            return Task.CompletedTask;
                        }
                },
                config
            );
        }
    }

    public record StorageExplorerDialogOptions(string Prefix = "/storage", bool IsMultiple = false)
    {
        public StorageItem[] Items { get; set; } = new StorageItem[0];
    }
}
