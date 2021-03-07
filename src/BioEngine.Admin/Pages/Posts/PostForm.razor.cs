using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AntDesign;
using BioEngine.Admin.Helpers;
using BioEngine.Admin.Shared;
using BioEngine.Core.Data;
using BioEngine.Core.Data.Entities;
using BioEngine.Core.Data.Entities.Abstractions;
using BioEngine.Core.Data.Entities.Blocks;
using BioEngine.Core.Data.Repositories;
using BioEngine.Core.Users;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tag = BioEngine.Core.Data.Entities.Tag;

namespace BioEngine.Admin.Pages.Posts
{
    public partial class PostForm
    {
        private string _oldTitle = string.Empty;
        private List<Section> Sections { get; set; } = new();
        private List<Tag> Tags { get; set; } = new();

        protected override async Task<PostFormModel> CreateFormModelAsync(Post post)
        {
            if (IsNew)
            {
                post.AuthorId = ScopedServices.GetRequiredService<ICurrentUserProvider>().CurrentUser!.Id;
            }

            _oldTitle = post.Title;
            Sections.AddRange((await ScopedServices.GetRequiredService<SectionsRepository>().GetAllAsync()).items);
            Tags.AddRange((await ScopedServices.GetRequiredService<TagsRepository>().GetAllAsync()).items);
            return new PostFormModel(post, Sections, Tags);
        }

        protected override Task OnCreatedAsync(Post entity)
        {
            NavigationManager.NavigateTo($"/Posts/{entity.Id}");
            return base.OnCreatedAsync(entity);
        }

        private void TitleChanged(KeyboardEventArgs args)
        {
            var title = FormModel.Title + args.Key;
            if (!string.IsNullOrEmpty(title))
            {
                FormModel.Url = SlugifyHelper.Slugify(title, _oldTitle, FormModel.Url);
                _oldTitle = FormModel.Title;
            }
            else
            {
                FormModel.Url = string.Empty;
            }

            Form?.Validate();
        }
    }

    public class PostFormModel : BaseFormModel<Post, Guid>, IBlocksItem
    {
        private readonly Post _post;
        private readonly IEnumerable<Section> _sections;
        private readonly IEnumerable<Tag> _tags;

        public PostFormModel(Post post, IEnumerable<Section> sections, IEnumerable<Tag> tags)
        {
            _post = post;
            Id = post.Id;
            Title = post.Title;
            Url = post.Url;
            Blocks = post.Blocks;
            Tags = post.Tags;
            Sections = post.Sections;
            _sections = sections;
            _tags = tags;
        }

        public Guid Id { get; set; }

        public Guid DummySectionId { get; set; }
        public Guid DummyTagId { get; set; }

        public IEnumerable<Guid> SectionIds
        {
            get => Sections.Select(s => s.Id);
            set
            {
                Sections = _sections.Where(s => value.Contains(s.Id)).ToList();
            }
        }

        public IEnumerable<Guid> TagIds
        {
            get => Tags.Select(s => s.Id);
            set
            {
                Tags = _tags.Where(s => value.Contains(s.Id)).ToList();
            }
        }

        public string Title { get; set; }
        public string Url { get; set; }
        public List<Section> Sections { get; set; }
        public List<ContentBlock> Blocks { get; set; }
        public List<Tag> Tags { get; set; }

        public object? GetId()
        {
            return Id;
        }

        public DateTimeOffset DateAdded { get; set; }
        public DateTimeOffset DateUpdated { get; set; }

        public override Post GetEntity()
        {
            _post.Title = Title;
            _post.Url = Url;
            _post.Sections = Sections;
            _post.Tags = Tags;
            _post.Blocks = Blocks;
            return _post;
        }
    }


    public class PostValidator : AbstractValidator<PostFormModel>
    {
        public PostValidator(BioDbContext dbContext)
        {
            RuleFor(e => e.Title).NotEmpty().WithMessage("Укажите заголовок").MaximumLength(1024).MinimumLength(5)
                .WithMessage("Заголовок должен быть от 5 до 1024 символов.");
            RuleFor(e => e.Url).NotEmpty().WithMessage("Укажите адрес");
            RuleFor(e => e.Sections)
                .NotEmpty()
                .WithMessage("Укажите разделы")
                .OverridePropertyName(nameof(PostFormModel.DummySectionId));
            RuleForEach(p => p.Blocks).AddBlockValidators();

            // RuleFor(e => e.Url).CustomAsync(async (url, context, _) =>
            // {
            //     if (context.InstanceToValidate is PostFormModel contentItem && !string.IsNullOrEmpty(url))
            //     {
            //         var count = await dbContext.Set<Post>().Where(p => p.Url == url && p.Id != contentItem.Id)
            //             .CountAsync();
            //         if (count > 0)
            //         {
            //             context.AddFailure(
            //                 $"Url {url} already taken");
            //         }
            //     }
            // });
            RuleFor(e => e.Blocks).NotEmpty();
        }
    }
}
