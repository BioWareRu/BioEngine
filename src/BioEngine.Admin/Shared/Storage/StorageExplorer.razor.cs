using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AntDesign;
using AntDesign.TableModels;
using BioEngine.Admin.Extensions;
using BioEngine.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Sitko.Core.Storage;
using Sitko.Core.Storage.ImgProxy;

namespace BioEngine.Admin.Shared.Storage
{
    using System.Threading;
    using Sitko.Core.Blazor.AntDesignComponents.Components;

    public partial class StorageExplorer
    {
        private IEnumerable<StorageNode> _selectedRows = new StorageNode[0];

        protected override async Task<(StorageNode[] items, int itemsCount)> GetDataAsync(
            LoadRequest<StorageNode> request, CancellationToken cancellationToken = default)
        {
            var items = (await Storage.GetDirectoryContentsAsync(GetStoragePath(CurrentPath), cancellationToken))
                .ToArray();
            var query = items.AsQueryable().OrderByDescending(n => n.Type == StorageNodeType.Directory);

            foreach (var sort in request.Sort)
            {
                query = sort.Operation(query);
            }

            return (query.ToArray(), items.Length);
        }

        private (string path, string name)[] Parts => StorageUploadExtensions.GetPathParts(CurrentPath);
        protected List<StorageItem> _storageItems { get; set; } = new();
        [Inject] private IImgProxyUrlGenerator<BRCStorageConfig> ImgProxyUrlGenerator { get; set; }
        [Inject] ModalService ModalService { get; set; }
        [Inject] IJSRuntime JsRuntime { get; set; }

        [Parameter] public string Prefix { get; set; }

        [Parameter] public string CurrentPath { get; set; }

        [Parameter] public bool UseNavigation { get; set; }

        [Parameter] public bool SelectMode { get; set; }

        [Parameter] public bool IsMultiple { get; set; }

        [Parameter] public EventCallback<StorageItem[]> OnItemsSelect { get; set; }

        [Inject] protected IStorage<BRCStorageConfig> Storage { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            await LoadDataAsync(string.IsNullOrEmpty(CurrentPath) ? "/" : CurrentPath);
        }

        private async Task LoadDataAsync(string path)
        {
            CurrentPath = path;
            await RefreshAsync();
        }

        private Task NavigateAsync(string path)
        {
            var currentPath = Path.Combine(CurrentPath, path);
            return LoadDataAsync(currentPath);
        }

        private async Task UploadFilesAsync(List<StorageItem> files)
        {
            _storageItems = new List<StorageItem>();
            await LoadDataAsync(CurrentPath);
        }

        private string GetStoragePath(string path)
        {
            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }

            return GetUrlPath(Prefix, path);
        }

        private Task OpenFilesDialogAsync()
        {
            var arg = new { id = "customFile" };
            return JsRuntime.InvokeVoidAsync(
                "FileUpload.open",
                arg).AsTask();
        }

        private async Task ShowFolderDialogAsync()
        {
            var options = new ModalOptions() { Title = "Новая папка" };

            var confirmRef = await ModalService.CreateModalAsync<FolderCreationDialog, string, string>(options, "");

            confirmRef.OnOk = async result =>
            {
                if (!string.IsNullOrEmpty(result))
                {
                    if (UseNavigation)
                    {
                        NavigationManager.NavigateTo(GetUrlPath(null, result));
                    }
                    else
                    {
                        await NavigateAsync(result);
                    }
                }
            };
        }

        private Task SelectItemsAsync() =>
            OnItemsSelect.InvokeAsync(_selectedRows.Select(r => r.StorageItem!).ToArray());

        private bool CanSelectItem(StorageNode item)
        {
            if (IsMultiple)
            {
                return true;
            }

            if (!_selectedRows.Any())
            {
                return true;
            }

            if (_selectedRows.Contains(item))
            {
                return true;
            }

            return false;
        }

        // private Task OnChange(QueryModel<StorageNode> queryModel) =>
        //     LoadData(queryModel.PageSize * (queryModel.PageIndex - 1), queryModel.PageSize, queryModel.SortModel);

        // private async Task LoadData(int skip = 0, int take = 20, IList<ITableSortModel> sortModel = null)
        // {
        //     await StartLoadingAsync();
        //     _items = await Storage.GetDirectoryContentsAsync(GetStoragePath(CurrentPath));
        //     Count = _items.Count();
        //
        //     var query = _items.AsQueryable().OrderByDescending(n => n.Type == StorageNodeType.Directory);
        //
        //     if (sortModel != null)
        //     {
        //         foreach (var sort in sortModel.Where(s => s.Sort is not null))
        //         {
        //             query = sort.ThenSort(query);
        //         }
        //     }
        //
        //     Items = query.Skip(skip).Take(take).ToList();
        //
        //     _loading = false;
        //     StateHasChanged();
        // }

        private async Task RefreshMetadataAsync()
        {
            await StartLoadingAsync();
            await Storage.RefreshDirectoryContentsAsync(GetStoragePath(CurrentPath));
            await RefreshAsync();
        }

        private string GetUrlPath(string? baseUrl, string path)
        {
            if (baseUrl is null)
            {
                var uri = new Uri(NavigationManager.Uri);
                baseUrl = uri.AbsolutePath;
            }

            if (path == "/")
            {
                return baseUrl;
            }

            return Path.Combine(baseUrl, path).Replace("\\", "/").Replace("//", "/");
        }
    }
}
