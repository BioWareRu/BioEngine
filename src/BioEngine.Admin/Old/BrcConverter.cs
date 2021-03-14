using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Admin.Old.Entities;
using BioEngine.Core;
using BioEngine.Core.Data;
using BioEngine.Core.Data.Entities;
using BioEngine.Core.Data.Entities.Blocks;
using BioEngine.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sitko.Core.Storage;

namespace BioEngine.Admin.Old
{
    public class BrcConverter
    {
        private readonly OldBrcContext _oldBrcContext;
        private readonly BioDbContext _bioDbContext;
        private readonly IStorage<BRCStorageConfig> _storage;
        private readonly ILogger<BrcConverter> _logger;

        public BrcConverter(OldBrcContext oldBrcContext, BioDbContext bioDbContext, IStorage<BRCStorageConfig> storage,
            ILogger<BrcConverter> logger)
        {
            _oldBrcContext = oldBrcContext;
            _bioDbContext = bioDbContext;
            _storage = storage;
            _logger = logger;
        }

        public async Task ConvertSitesAsync()
        {
            var oldSites = await _oldBrcContext.Sites.ToListAsync();
            foreach (var oldSite in oldSites)
            {
                _logger.LogInformation("Converting site {Site}", oldSite.Title);
                var site = await _bioDbContext.Sites.FirstOrDefaultAsync(s => s.Id == oldSite.Id);
                if (site is null)
                {
                    site = new Site {Id = oldSite.Id};
                    await _bioDbContext.Sites.AddAsync(site);
                }

                site.Title = oldSite.Title;
                site.Url = oldSite.Url;
                site.DateAdded = oldSite.DateAdded;
                site.DateUpdated = oldSite.DateUpdated;
                _logger.LogInformation("Site converting done");
            }

            await _bioDbContext.SaveChangesAsync();
            _logger.LogInformation("All sites converting done");
        }

        public async Task ConvertSectionsAsync()
        {
            var sites = await _bioDbContext.Sites.ToListAsync();
            var oldSections = await _oldBrcContext.Sections.ToListAsync();
            foreach (var oldSection in oldSections)
            {
                _logger.LogInformation("Converting section {Title}", oldSection.Title);
                var section = await _bioDbContext.Sections.Where(s => s.Id == oldSection.Id).Include(s => s.Sites)
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
                        var gameData = new GameData {Hashtag = oldSection.Data.Hashtag, HeaderPicture = headerPicture};
                        sectionData = gameData;
                        newSection = new Game {Id = oldSection.Id, Data = gameData};

                        break;
                    case "developersection":
                        var developerData = new DeveloperData
                        {
                            Hashtag = oldSection.Data.Hashtag, HeaderPicture = headerPicture
                        };
                        sectionData = developerData;
                        newSection = new Developer() {Id = oldSection.Id, Data = developerData};
                        break;
                    case "topicsection":
                        var topicData = new TopicData
                        {
                            Hashtag = oldSection.Data.Hashtag, HeaderPicture = headerPicture
                        };
                        sectionData = topicData;
                        newSection = new Topic() {Id = oldSection.Id, Data = topicData};
                        break;
                    default:
                        _logger.LogError("Unknown section type: {Type}", oldSection.Type);
                        continue;
                }

                if (section is null)
                {
                    section = newSection;
                    await _bioDbContext.Sections.AddAsync(newSection);
                }

                section.Title = oldSection.Title;
                section.DateAdded = oldSection.DateAdded;
                section.DateUpdated = oldSection.DateAdded;
                section.DatePublished = oldSection.DatePublished;
                section.Url = oldSection.Url;
                section.IsPublished = oldSection.IsPublished;
                section.ParentId = oldSection.ParentId;
                section.Sites.Clear();
                section.Sites.AddRange(sites.Where(s => oldSection.SiteIds.Contains(s.Id)));
                var blocks = await _oldBrcContext.ContentBlocks.Where(b => b.ContentId == oldSection.Id).ToListAsync();
                var newBlocks = new List<ContentBlock>();
                foreach (var oldContentBlock in blocks)
                {
                    newBlocks.Add(await ConvertBlockAsync(oldContentBlock));
                }

                section.Blocks = newBlocks;
                section.SetData(sectionData);
                _logger.LogInformation("Converting section {Title} done", oldSection.Title);
            }

            await _bioDbContext.SaveChangesAsync();
            _logger.LogInformation("Converting all sections done");
        }

        public async Task ConvertTagsAsync()
        {
            var oldTags = await _oldBrcContext.Tags.ToListAsync();
            foreach (var oldTag in oldTags)
            {
                _logger.LogInformation("Converting tag {Title}", oldTag.Title);
                var tag = await _bioDbContext.Tags.Where(s => s.Id == oldTag.Id).FirstOrDefaultAsync();
                if (tag is null)
                {
                    tag = new Tag {Id = oldTag.Id};
                    await _bioDbContext.Tags.AddAsync(tag);
                }

                tag.Title = oldTag.Title;
                tag.DateAdded = oldTag.DateAdded;
                tag.DateUpdated = oldTag.DateUpdated;
                _logger.LogInformation("Converting tag {Title} done", oldTag.Title);
            }

            await _bioDbContext.SaveChangesAsync();
            _logger.LogInformation("Converting all tags done");
        }

        public async Task ConvertPostsAsync()
        {
            var sites = await _bioDbContext.Sites.ToListAsync();
            var sections = await _bioDbContext.Sections.ToListAsync();
            var tags = await _bioDbContext.Tags.ToListAsync();
            var oldPosts = await _oldBrcContext.Posts.ToListAsync();
            foreach (var oldPost in oldPosts)
            {
                _logger.LogInformation("Converting post {Title}", oldPost.Title);
                var post = await _bioDbContext.Posts.Where(s => s.Id == oldPost.Id)
                    .Include(s => s.Sites)
                    .Include(s => s.Sections)
                    .Include(s => s.Tags)
                    .FirstOrDefaultAsync();
                if (post is null)
                {
                    post = new Post {Id = oldPost.Id};
                    await _bioDbContext.Posts.AddAsync(post);
                }

                post.Title = oldPost.Title;
                post.Url = oldPost.Url;
                post.DateAdded = oldPost.DateAdded;
                post.DateUpdated = oldPost.DateUpdated;
                post.DatePublished = oldPost.DatePublished;
                post.IsPublished = oldPost.IsPublished;
                post.AuthorId = oldPost.AuthorId;

                post.Sites.Clear();
                post.Sites.AddRange(sites.Where(s => oldPost.SiteIds.Contains(s.Id)));
                post.Sections.Clear();
                post.Sections.AddRange(sections.Where(s => oldPost.SectionIds.Contains(s.Id)));
                post.Tags.Clear();
                post.Tags.AddRange(tags.Where(s => oldPost.TagIds.Contains(s.Id)));

                var blocks = await _oldBrcContext.ContentBlocks.Where(b => b.ContentId == oldPost.Id).ToListAsync();
                var newBlocks = new List<ContentBlock>();
                foreach (var oldContentBlock in blocks)
                {
                    newBlocks.Add(await ConvertBlockAsync(oldContentBlock));
                }

                post.Blocks = newBlocks;
                _logger.LogInformation("Converting post {Title} done", oldPost.Title);
                await _bioDbContext.SaveChangesAsync();
            }

            
            _logger.LogInformation("Converting all posts done");
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
            var metadata = new StorageItemMetadata {Type = StorageItemType.File};
            if (oldStorageItem.Type == OldStorageItemType.Picture && oldStorageItem.PictureInfo is not null)
            {
                metadata.Type = StorageItemType.Image;
                metadata.ImageMetadata = new StorageItemImageMetadata
                {
                    Height = (int)oldStorageItem.PictureInfo.VerticalResolution,
                    Width = (int)oldStorageItem.PictureInfo.HorizontalResolution
                };
            }

            return await _storage.UpdateMetaDataAsync(storageItem, oldStorageItem.FileName, metadata);
        }

        private async Task<ContentBlock> ConvertBlockAsync(OldContentBlock oldContentBlock)
        {
            ContentBlock block;
            switch (oldContentBlock.Type)
            {
                case "quoteblock":
                    var oldQuoteData = JsonConvert.DeserializeObject<OldQuoteBlockData>(oldContentBlock.Data);
                    block = new QuoteBlock
                    {
                        Data = new QuoteBlockData
                        {
                            Author = oldQuoteData.Author,
                            Link = oldQuoteData.Link,
                            Picture = await ConvertStorageItemAsync(oldQuoteData.Picture),
                            Text = oldQuoteData.Text
                        }
                    };
                    break;
                case "cutblock":
                    var oldCutData = JsonConvert.DeserializeObject<OldCutBlockData>(oldContentBlock.Data);
                    block = new CutBlock {Data = new CutBlockData {ButtonText = oldCutData.ButtonText}};
                    break;
                case "twitchblock":
                    var oldTwitchData = JsonConvert.DeserializeObject<OldTwitchBlockData>(oldContentBlock.Data);
                    block = new TwitchBlock
                    {
                        Data = new TwitchBlockData
                        {
                            ChannelId = oldTwitchData.ChannelId,
                            CollectionId = oldTwitchData.CollectionId,
                            VideoId = oldTwitchData.VideoId
                        }
                    };
                    break;
                case "twitterblock":
                    var oldTwitterData = JsonConvert.DeserializeObject<OldTwitterBlockData>(oldContentBlock.Data);
                    block = new TwitterBlock
                    {
                        Data = new TwitterBlockData
                        {
                            TweetAuthor = oldTwitterData.TweetAuthor, TweetId = oldTwitterData.TweetId,
                        }
                    };
                    break;
                case "pictureblock":
                    var oldPictureData = JsonConvert.DeserializeObject<OldPictureBlockData>(oldContentBlock.Data);
                    block = new PictureBlock
                    {
                        Data = new PictureBlockData
                        {
                            Url = oldPictureData.Url,
                            Picture = await ConvertStorageItemAsync(oldPictureData.Picture)
                        }
                    };
                    break;
                case "galleryblock":
                    var oldGalleryData = JsonConvert.DeserializeObject<OldGalleryBlockData>(oldContentBlock.Data);
                    var newItems = new ValueCollection<StorageItem>();
                    foreach (var oldStorageItem in oldGalleryData.Pictures)
                    {
                        newItems.Add(await ConvertStorageItemAsync(oldStorageItem));
                    }

                    block = new GalleryBlock {Data = new GalleryBlockData {Pictures = newItems}};
                    break;
                case "textblock":
                    var oldTextData = JsonConvert.DeserializeObject<OldTextBlockData>(oldContentBlock.Data);
                    block = new TextBlock {Data = new TextBlockData {Text = oldTextData.Text}};
                    break;
                case "fileblock":
                    var oldFileData = JsonConvert.DeserializeObject<OldFileBlockData>(oldContentBlock.Data);
                    block = new FileBlock
                    {
                        Data = new FileBlockData {File = await ConvertStorageItemAsync(oldFileData.File)}
                    };
                    break;
                case "youtubeblock":
                    var oldYoutubeData = JsonConvert.DeserializeObject<OldYoutubeBlockData>(oldContentBlock.Data);
                    block = new YoutubeBlock {Data = new YoutubeBlockData {YoutubeId = oldYoutubeData.YoutubeId}};
                    break;
                case "iframeblock":
                    var oldIframeData = JsonConvert.DeserializeObject<OldIframeBlockData>(oldContentBlock.Data);
                    block = new IframeBlock
                    {
                        Data = new IframeBlockData
                        {
                            Height = oldIframeData.Height, Width = oldIframeData.Width, Src = oldIframeData.Src
                        }
                    };
                    break;
                default:
                    throw new Exception($"Unknown block type {oldContentBlock.Type}");
            }

            block.Position = oldContentBlock.Position;
            block.Id = oldContentBlock.Id;

            return block;
        }
    }
}
