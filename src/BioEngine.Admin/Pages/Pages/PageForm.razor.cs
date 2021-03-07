using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Admin.Pages.Sites;
using BioEngine.Admin.Shared;
using BioEngine.Core.Data.Entities;
using BioEngine.Core.Data.Entities.Abstractions;
using BioEngine.Core.Data.Entities.Blocks;
using BioEngine.Core.Data.Repositories;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BioEngine.Admin.Pages.Pages
{
    public partial class PageForm
    {
        private List<Site> Sites { get; set; } = new();

        protected override async Task<PageFormModel> CreateFormModelAsync(Page entity)
        {
            Sites.AddRange((await ScopedServices.GetRequiredService<SitesRepository>().GetAllAsync()).items);
            return new PageFormModel(entity, Sites);
        }

        protected override Task OnCreatedAsync(Page entity)
        {
            NavigationManager.NavigateTo($"/Pages/{entity.Id}");
            return base.OnCreatedAsync(entity);
        }
    }

    public class PageFormModel : BaseFormModel<Page, Guid>, IContentEntity
    {
        private readonly Page _page;
        private readonly IEnumerable<Site> _sites;
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool IsPublished { get; set; }
        public DateTimeOffset? DatePublished { get; set; }
        public string Url { get; set; }
        public Guid DummySiteId { get; set; }

        public PageFormModel(Page page, IEnumerable<Site> sites)
        {
            Id = page.Id;
            Title = page.Title;
            Url = page.Url;
            Blocks = page.Blocks;
            Sites = page.Sites;
            _page = page;
            _sites = sites;
        }

        public IEnumerable<Guid> SiteIds
        {
            get => Sites.Select(s => s.Id);
            set
            {
                Sites = _sites.Where(s => value.Contains(s.Id)).ToList();
            }
        }

        public override Page GetEntity()
        {
            _page.Title = Title;
            _page.Url = Url;
            _page.Sites = Sites;
            _page.Blocks = Blocks;
            return _page;
        }

        public object? GetId()
        {
            return _page.Id;
        }

        public DateTimeOffset DateAdded { get; set; }
        public DateTimeOffset DateUpdated { get; set; }
        public List<Site> Sites { get; set; }
        public List<ContentBlock> Blocks { get; set; }
    }

    public class PageFormValidator : AbstractValidator<PageFormModel>
    {
        public PageFormValidator()
        {
            RuleFor(e => e.Title).NotEmpty().WithMessage("Укажите заголовок").MaximumLength(1024).MinimumLength(5)
                .WithMessage("Заголовок должен быть от 5 до 1024 символов.");
            RuleFor(e => e.Url).NotEmpty().WithMessage("Укажите адрес");
            RuleFor(e => e.Sites)
                .NotEmpty()
                .WithMessage("Укажите сайты")
                .OverridePropertyName(nameof(PageFormModel.DummySiteId));
            RuleForEach(p => p.Blocks).AddBlockValidators();
            RuleFor(e => e.Blocks).NotEmpty();
        }
    }
}
