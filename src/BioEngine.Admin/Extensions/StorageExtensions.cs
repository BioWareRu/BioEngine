using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AntDesign;
using BioEngine.Admin.Shared.Storage;
using BioEngine.Core;
using BioEngine.Core.Extensions;
using Sitko.Core.Blazor.FileUpload;
using Sitko.Core.Storage;
using Image = SixLabors.ImageSharp.Image;

namespace BioEngine.Admin.Extensions
{
    using Sitko.Core.App.Collections;

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

        public static Task<object> GenerateMetadataAsync(FileUploadRequest request, FileStream stream)
        {
            var metaData = new StorageItemMetadata();
            var extension = Path.GetExtension(request.Name).ToLowerInvariant();
            if (_imageExtensions.Contains(extension))
            {
                stream.Position = 0;
                metaData.Type = StorageItemType.Image;
                using Image image = Image.Load(stream);
                metaData.ImageMetadata = new StorageItemImageMetadata {Height = image.Height, Width = image.Width};
                stream.Position = 0;
            }

            return Task.FromResult<object>(metaData);
        }

        public static (string path, string name)[] GetPathParts(string path)
        {
            var parts = new List<(string path, string name)> {("/", "Начало")};
            if (!string.IsNullOrEmpty(path) && path != "/")
            {
                string[] directories = path.Split('/');

                string previousEntry = string.Empty;
                foreach (string dir in directories)
                {
                    string newEntry = previousEntry + '/' + dir;
                    if (!string.IsNullOrEmpty(newEntry))
                    {
                        if (!newEntry.Equals(Convert.ToString('/'),
                            StringComparison.OrdinalIgnoreCase))
                        {
                            parts.Add((newEntry.Trim('/'),
                                dir.Replace("/", "")));
                            previousEntry = newEntry;
                        }
                    }
                }
            }

            return parts.ToArray();
        }

        public static async Task<StorageItem[]> SelectStorageItemsAsync(this ModalService modalService,
            string prefix = "storage", bool isMultiple = false)
        {
            var resultTask = new TaskCompletionSource<StorageItem[]>();
            var config = new StorageExplorerDialogOptions(prefix, isMultiple);
            var modalRef = await modalService
                .CreateModalAsync<StorageExplorerDialog, StorageExplorerDialogOptions, StorageItem[]>(
                    new ModalOptions
                    {
                        Footer = null,
                        Title = isMultiple ? "Выбор файлов" : "Выбор файла",
                        Width = "60%",
                        DestroyOnClose = true,
                        MaskClosable = false
                    },
                    config
                );
            modalRef.OnOk = items =>
            {
                resultTask.SetResult(items);
                return Task.CompletedTask;
            };
            modalRef.OnCancel = _ =>
            {
                resultTask.SetResult(Array.Empty<StorageItem>());
                return Task.CompletedTask;
            };
            var result = await resultTask.Task;
            return result;
        }
    }

    public record StorageExplorerDialogOptions(string Prefix = "/storage", bool IsMultiple = false)
    {
    }
}
