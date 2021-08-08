using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntDesign.Core.Helpers.MemberPath;
using BioEngine.Admin.Old.Entities;
using BioEngine.Core;
using BioEngine.Core.Data;
using BioEngine.Core.Data.Entities;
using BioEngine.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sitko.Blockly;
using Sitko.Blockly.Blocks;
using Sitko.Core.App.Collections;
using Sitko.Core.Storage;

namespace BioEngine.Admin.Old
{
    public class BrcConverter
    {
        private readonly OldBrcContext oldBrcContext;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IStorage<BRCStorageConfig> storage;
        private readonly ILogger<BrcConverter> logger;

        public BrcConverter(OldBrcContext oldBrcContext, IServiceScopeFactory serviceScopeFactory,
            IStorage<BRCStorageConfig> storage,
            ILogger<BrcConverter> logger)
        {
            this.oldBrcContext = oldBrcContext;
            this.serviceScopeFactory = serviceScopeFactory;
            this.storage = storage;
            this.logger = logger;
        }

        public async Task ConvertSitesAsync(Func<int, int, string, Task> trackProgress)
        {
            var oldSites = await oldBrcContext.Sites.ToListAsync();
            var processed = 0;
            foreach (var oldSite in oldSites)
            {
                logger.LogInformation("Converting site {Site}", oldSite.Title);
                using var scope = serviceScopeFactory.CreateScope();
                var bioDbContext = scope.ServiceProvider.GetRequiredService<BioDbContext>();
                var site = await bioDbContext.Sites.FirstOrDefaultAsync(s => s.Id == oldSite.Id);
                if (site is null)
                {
                    site = new Site { Id = oldSite.Id };
                    await bioDbContext.Sites.AddAsync(site);
                }

                site.Title = oldSite.Title;
                site.Url = oldSite.Url;
                site.DateAdded = oldSite.DateAdded;
                site.DateUpdated = oldSite.DateUpdated;
                await trackProgress(oldSites.Count, ++processed, site.Title);
                await bioDbContext.SaveChangesAsync();
                logger.LogInformation("Site converting done");
            }

            logger.LogInformation("All sites converting done");
        }

        public async Task ConvertSectionsAsync(Func<int, int, string, Task> trackProgress)
        {
            var oldSections = await oldBrcContext.Sections.ToListAsync();
            var processed = 0;
            foreach (var oldSection in oldSections)
            {
                logger.LogInformation("Converting section {Title}", oldSection.Title);
                using var scope = serviceScopeFactory.CreateScope();
                var bioDbContext = scope.ServiceProvider.GetRequiredService<BioDbContext>();
                var section = await bioDbContext.Sections.Where(s => s.Id == oldSection.Id).Include(s => s.Sites)
                    .FirstOrDefaultAsync();
                SectionData sectionData;
                Section newSection;
                StorageItem? headerPicture = null;
                if (oldSection.Data.HeaderPicture is not null)
                {
                    headerPicture = await ConvertStorageItemAsync(oldSection.Data.HeaderPicture);
                }

                switch (oldSection.Type)
                {
                    case "gamesection":
                        var gameData = new GameData
                        {
                            Hashtag = oldSection.Data.Hashtag, HeaderPicture = headerPicture
                        };
                        sectionData = gameData;
                        newSection = new Game { Id = oldSection.Id, Data = gameData };

                        break;
                    case "developersection":
                        var developerData = new DeveloperData
                        {
                            Hashtag = oldSection.Data.Hashtag, HeaderPicture = headerPicture
                        };
                        sectionData = developerData;
                        newSection = new Developer() { Id = oldSection.Id, Data = developerData };
                        break;
                    case "topicsection":
                        var topicData = new TopicData
                        {
                            Hashtag = oldSection.Data.Hashtag, HeaderPicture = headerPicture
                        };
                        sectionData = topicData;
                        newSection = new Topic() { Id = oldSection.Id, Data = topicData };
                        break;
                    default:
                        logger.LogError("Unknown section type: {Type}", oldSection.Type);
                        continue;
                }

                if (section is null)
                {
                    section = newSection;
                    await bioDbContext.Sections.AddAsync(newSection);
                }

                section.Title = oldSection.Title;
                section.DateAdded = oldSection.DateAdded;
                section.DateUpdated = oldSection.DateAdded;
                section.DatePublished = oldSection.DatePublished;
                section.Url = oldSection.Url;
                section.IsPublished = oldSection.IsPublished;
                section.ParentId = oldSection.ParentId;
                section.Sites.Clear();
                section.Sites.AddRange(await bioDbContext.Sites.Where(s => oldSection.SiteIds.Contains(s.Id))
                    .ToListAsync());
                var blocks = await oldBrcContext.ContentBlocks.Where(b => b.ContentId == oldSection.Id).ToListAsync();
                var newBlocks = new OrderedCollection<ContentBlock>();
                foreach (var oldContentBlock in blocks.OrderBy(b => b.Position))
                {
                    var block = await ConvertBlockAsync(oldContentBlock);
                    if (block is not null)
                    {
                        newBlocks.AddItem(block);
                    }
                }

                section.Blocks = newBlocks.ToList();
                section.SetData(sectionData);
                await trackProgress(oldSections.Count, ++processed, section.Title);
                await bioDbContext.SaveChangesAsync();
                logger.LogInformation("Converting section {Title} done", oldSection.Title);
            }

            logger.LogInformation("Converting all sections done");
        }

        public async Task ConvertTagsAsync(Func<int, int, string, Task> trackProgress)
        {
            var oldTags = await oldBrcContext.Tags.ToListAsync();
            var processed = 0;
            foreach (var oldTag in oldTags)
            {
                logger.LogInformation("Converting tag {Title}", oldTag.Title);
                using var scope = serviceScopeFactory.CreateScope();
                var bioDbContext = scope.ServiceProvider.GetRequiredService<BioDbContext>();
                var tag = await bioDbContext.Tags.Where(s => s.Id == oldTag.Id).FirstOrDefaultAsync();
                if (tag is null)
                {
                    tag = new Tag { Id = oldTag.Id };
                    await bioDbContext.Tags.AddAsync(tag);
                }

                tag.Title = oldTag.Title;
                tag.DateAdded = oldTag.DateAdded;
                tag.DateUpdated = oldTag.DateUpdated;
                await trackProgress(oldTags.Count, ++processed, tag.Title);
                await bioDbContext.SaveChangesAsync();
                logger.LogInformation("Converting tag {Title} done", oldTag.Title);
            }

            logger.LogInformation("Converting all tags done");
        }

        public async Task ConvertPostsAsync(Func<int, int, string, Task> trackProgress)
        {
            var oldPosts = await oldBrcContext.Posts.ToListAsync();
            var processed = 0;
            foreach (var oldPost in oldPosts)
            {
                await ConvertPost(oldPost);
                await trackProgress(oldPosts.Count, ++processed, oldPost.Title);
            }

            logger.LogInformation("Converting all posts done");
        }

        public async Task ConvertPostAsync(Guid postId)
        {
            var oldPost = await oldBrcContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            if (oldPost is not null)
            {
                await ConvertPost(oldPost);
            }

            logger.LogInformation("Converting post done");
        }

        private async Task ConvertPost(OldPost oldPost)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var bioDbContext = scope.ServiceProvider.GetRequiredService<BioDbContext>();
            logger.LogInformation("Converting post {Title}", oldPost.Title);
            var post = await bioDbContext.Posts.Where(s => s.Id == oldPost.Id)
                .Include(s => s.Sites)
                .Include(s => s.Sections)
                .Include(s => s.Tags)
                .FirstOrDefaultAsync();
            if (post is null)
            {
                post = new Post { Id = oldPost.Id };
                await bioDbContext.Posts.AddAsync(post);
            }

            post.Title = oldPost.Title;
            post.Url = oldPost.Url;
            post.DateAdded = oldPost.DateAdded;
            post.DateUpdated = oldPost.DateUpdated;
            post.DatePublished = oldPost.DatePublished;
            post.IsPublished = oldPost.IsPublished;
            post.AuthorId = oldPost.AuthorId;

            post.Sites.Clear();
            post.Sites.AddRange(await bioDbContext.Sites.Where(s => oldPost.SiteIds.Contains(s.Id)).ToListAsync());
            post.Sections.Clear();
            post.Sections.AddRange(await bioDbContext.Sections.Where(s => oldPost.SectionIds.Contains(s.Id))
                .ToListAsync());
            post.Tags.Clear();
            post.Tags.AddRange(await bioDbContext.Tags.Where(s => oldPost.TagIds.Contains(s.Id)).ToListAsync());

            var blocks = await oldBrcContext.ContentBlocks.Where(b => b.ContentId == oldPost.Id).ToListAsync();
            var newBlocks = new OrderedCollection<ContentBlock>();
            foreach (var oldContentBlock in blocks.OrderBy(b => b.Position))
            {
                var block = await ConvertBlockAsync(oldContentBlock);
                if (block is not null)
                {
                    newBlocks.AddItem(block);
                }
            }

            post.Blocks = newBlocks.ToList();
            logger.LogInformation("Converting post {Title} done", oldPost.Title);
            await bioDbContext.SaveChangesAsync();
        }

        private async Task<StorageItem?> ConvertStorageItemAsync(OldStorageItem? oldStorageItem)
        {
            if (oldStorageItem is null)
            {
                return null;
            }

            var storageItem = new StorageItem
            {
                Path = oldStorageItem.Path,
                FileName = oldStorageItem.FileName,
                FilePath = oldStorageItem.FilePath,
                FileSize = oldStorageItem.FileSize,
                LastModified = oldStorageItem.DateUpdated
            };
            var metadata = new StorageItemMetadata { Type = StorageItemType.File };
            if (oldStorageItem.Type == OldStorageItemType.Picture && oldStorageItem.PictureInfo is not null)
            {
                metadata.Type = StorageItemType.Image;
                metadata.ImageMetadata = new StorageItemImageMetadata
                {
                    Height = (int)oldStorageItem.PictureInfo.VerticalResolution,
                    Width = (int)oldStorageItem.PictureInfo.HorizontalResolution
                };
            }

            return await storage.UpdateMetaDataAsync(storageItem, oldStorageItem.FileName, metadata);
        }

        private async Task<ContentBlock?> ConvertBlockAsync(OldContentBlock oldContentBlock)
        {
            ContentBlock block;
            switch (oldContentBlock.Type)
            {
                case "quoteblock":
                    var oldQuoteData = JsonConvert.DeserializeObject<OldQuoteBlockData>(oldContentBlock.Data)!;
                    block = new QuoteBlock
                    {
                        Author = oldQuoteData.Author,
                        Link = oldQuoteData.Link,
                        Picture = await ConvertStorageItemAsync(oldQuoteData.Picture),
                        Text = oldQuoteData.Text
                    };
                    break;
                case "cutblock":
                    var oldCutData = JsonConvert.DeserializeObject<OldCutBlockData>(oldContentBlock.Data)!;
                    block = new CutBlock { ButtonText = oldCutData.ButtonText };
                    break;
                case "twitchblock":
                    var oldTwitchData = JsonConvert.DeserializeObject<OldTwitchBlockData>(oldContentBlock.Data)!;
                    block = new TwitchBlock
                    {
                        ChannelId = oldTwitchData.ChannelId,
                        CollectionId = oldTwitchData.CollectionId,
                        VideoId = oldTwitchData.VideoId
                    };
                    break;
                case "twitterblock":
                    var oldTwitterData = JsonConvert.DeserializeObject<OldTwitterBlockData>(oldContentBlock.Data)!;
                    block = new TwitterBlock
                    {
                        TweetAuthor = oldTwitterData.TweetAuthor, TweetId = oldTwitterData.TweetId
                    };
                    break;
                case "pictureblock":
                    var oldPictureData = JsonConvert.DeserializeObject<OldPictureBlockData>(oldContentBlock.Data)!;
                    var picture = await ConvertStorageItemAsync(oldPictureData.Picture!);
                    if (picture is null)
                    {
                        logger.LogError("Can't download item {FilePath}", oldPictureData.Picture?.FilePath);
                        return null;
                    }

                    block = new GalleryBlock
                    {
                        Pictures = new ValueCollection<StorageItem>(
                            new List<StorageItem> { picture })
                    };
                    break;
                case "galleryblock":
                    var oldGalleryData = JsonConvert.DeserializeObject<OldGalleryBlockData>(oldContentBlock.Data)!;
                    var newItems = new ValueCollection<StorageItem>();
                    foreach (var oldStorageItem in oldGalleryData.Pictures)
                    {
                        var galleryItem = await ConvertStorageItemAsync(oldStorageItem);
                        if (galleryItem is not null)
                        {
                            newItems.Add(galleryItem);
                        }
                    }

                    block = new GalleryBlock { Pictures = newItems };
                    break;
                case "textblock":
                    var oldTextData = JsonConvert.DeserializeObject<OldTextBlockData>(oldContentBlock.Data)!;
                    block = new TextBlock { Text = oldTextData.Text };
                    break;
                case "fileblock":
                    var oldFileData = JsonConvert.DeserializeObject<OldFileBlockData>(oldContentBlock.Data)!;
                    var item = await ConvertStorageItemAsync(oldFileData.File);
                    if (item is null)
                    {
                        logger.LogError("Can't download item {FilePath}", oldFileData.File?.FilePath);
                        return null;
                    }

                    block = new FilesBlock { Files = new ValueCollection<StorageItem>(new List<StorageItem> { item }) };
                    break;
                case "youtubeblock":
                    var oldYoutubeData = JsonConvert.DeserializeObject<OldYoutubeBlockData>(oldContentBlock.Data)!;
                    block = new YoutubeBlock { YoutubeId = oldYoutubeData.YoutubeId };
                    break;
                case "iframeblock":
                    var oldIframeData = JsonConvert.DeserializeObject<OldIframeBlockData>(oldContentBlock.Data)!;
                    block = new IframeBlock { Src = oldIframeData.Src };
                    break;
                default:
                    throw new InvalidPathException($"Unknown block type {oldContentBlock.Type}");
            }

            block.Position = oldContentBlock.Position;
            block.Id = oldContentBlock.Id;

            return block;
        }
    }
}
