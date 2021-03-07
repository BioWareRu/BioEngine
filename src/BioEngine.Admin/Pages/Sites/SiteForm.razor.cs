using System;
using System.Threading.Tasks;
using BioEngine.Admin.Shared;
using BioEngine.Core.Data.Entities;
using FluentValidation;

namespace BioEngine.Admin.Pages.Sites
{
    public partial class SiteForm
    {
        protected override Task<SiteFormModel> CreateFormModelAsync(Site entity)
        {
            return Task.FromResult(new SiteFormModel(entity));
        }
        
        protected override Task OnCreatedAsync(Site entity)
        {
            NavigationManager.NavigateTo($"/Sites/{entity.Id}");
            return base.OnCreatedAsync(entity);
        }
    }

    public class SiteFormModel : BaseFormModel<Site, Guid>
    {
        private readonly Site _site;
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }

        public SiteFormModel(Site site)
        {
            Id = site.Id;
            Title = site.Title;
            Url = site.Url;
            _site = site;
        }

        public override Site GetEntity()
        {
            _site.Title = Title;
            _site.Url = Url;
            return _site;
        }
    }

    public class SiteFormValidator : AbstractValidator<SiteFormModel>
    {
        public SiteFormValidator()
        {
            RuleFor(e => e.Title).NotEmpty().WithMessage("Укажите заголовок")
                .MaximumLength(1024).MinimumLength(5)
                .WithMessage("Заголовок должен быть от 5 до 1024 символов.");
            RuleFor(e => e.Url).NotEmpty().WithMessage("Укажите адрес")
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .WithMessage("Значение должно быть ссылкой");
            ;
        }
    }
}
