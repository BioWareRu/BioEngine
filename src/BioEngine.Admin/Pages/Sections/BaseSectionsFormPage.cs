using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Admin.Helpers;
using BioEngine.Admin.Pages.Sections.Developers;
using BioEngine.Admin.Shared;
using BioEngine.Core.Data.Entities;
using BioEngine.Core.Data.Entities.Abstractions;
using BioEngine.Core.Data.Entities.Blocks;
using BioEngine.Core.Data.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Sitko.Core.Repository;
using Sitko.Core.Storage;

namespace BioEngine.Admin.Pages.Sections
{
    public abstract class
        BaseSectionsFormPage<TEntity, TEntityData, TFormModel> : BasePublishableFormPage<TEntity, Guid, TFormModel>
        where TEntity : Section<TEntityData>, IEntity<Guid>, new()
        where TFormModel : BaseSectionFormModel<TEntity, TEntityData>
        where TEntityData : SectionData, new()
    {
        protected List<Site> Sites { get; set; } = new();
        private string _oldTitle = string.Empty;

        protected override async Task<TFormModel> CreateFormModelAsync(TEntity entity)
        {
            _oldTitle = entity.Title;
            Sites.AddRange((await ScopedServices.GetRequiredService<SitesRepository>().GetAllAsync()).items);
            return (Activator.CreateInstance(typeof(TFormModel), entity, Sites) as TFormModel)!;
        }

        protected void TitleChanged(KeyboardEventArgs args)
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

    public abstract class BaseSectionFormModel<TSection, TSectionData> : BaseFormModel<TSection, Guid>, IContentEntity
        where TSection : Section<TSectionData>, IEntity<Guid> where TSectionData : SectionData, new()
    {
        public Guid Id { get; set; }
        public DateTimeOffset DateAdded { get; set; }
        public DateTimeOffset DateUpdated { get; set; }
        public List<Site> Sites { get; set; }
        public string Url { get; set; }
        public List<ContentBlock> Blocks { get; set; }
        public bool IsPublished { get; set; }
        public DateTimeOffset? DatePublished { get; set; }
        public string Title { get; set; }
        public string? HashTag { get; set; }
        public StorageItem? HeaderPicture { get; set; }
        protected TSection _section;
        private readonly IEnumerable<Site> _sites;

        public Guid DummySiteId { get; set; }

        protected BaseSectionFormModel(TSection section, IEnumerable<Site> sites)
        {
            Id = section.Id;
            Title = section.Title;
            Url = section.Url;
            Blocks = section.Blocks;
            Sites = section.Sites;
            HashTag = section.Data.Hashtag;
            HeaderPicture = section.Data.HeaderPicture;
            _section = section;
            _sites = sites;
        }

        public override TSection GetEntity()
        {
            _section.Title = Title;
            _section.Url = Url;
            _section.Sites = Sites;
            _section.Blocks = Blocks;
            _section.Data.Hashtag = HashTag;
            _section.Data.HeaderPicture = HeaderPicture;
            FillSection();
            return _section;
        }

        protected virtual void FillSection()
        {
        }

        public object? GetId()
        {
            return _section.Id;
        }

        public IEnumerable<Guid> SiteIds
        {
            get => Sites.Select(s => s.Id);
            set
            {
                Sites = _sites.Where(s => value.Contains(s.Id)).ToList();
            }
        }
    }

    public abstract class
        BaseSectionFormValidator<TFormModel, TSection, TSectionData> : AbstractValidator<
            TFormModel>
        where TFormModel : BaseSectionFormModel<TSection, TSectionData>
        where TSection : Section<TSectionData>
        where TSectionData : SectionData, new()
    {
        protected BaseSectionFormValidator()
        {
            RuleFor(e => e.Title).NotEmpty().WithMessage("Укажите заголовок").MaximumLength(1024).MinimumLength(5)
                .WithMessage("Заголовок должен быть от 5 до 1024 символов.");
            RuleFor(e => e.Url).NotEmpty().WithMessage("Укажите адрес");
            RuleFor(e => e.Sites)
                .NotEmpty()
                .WithMessage("Укажите сайты")
                .OverridePropertyName(nameof(DeveloperFormModel.DummySiteId));
            RuleForEach(p => p.Blocks).AddBlockValidators();
            RuleFor(e => e.Blocks).NotEmpty();
        }
    }
}
