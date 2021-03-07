using System;
using System.Threading.Tasks;
using BioEngine.Admin.Shared;
using BioEngine.Core.Data.Entities;
using FluentValidation;

namespace BioEngine.Admin.Pages.Tags
{
    public partial class TagForm
    {
        protected override Task<TagFormModel> CreateFormModelAsync(Tag entity)
        {
            return Task.FromResult(new TagFormModel(entity));
        }

        protected override Task OnCreatedAsync(Tag entity)
        {
            NavigationManager.NavigateTo($"/Tags/{entity.Id}");
            return base.OnCreatedAsync(entity);
        }
    }

    public class TagFormModel : BaseFormModel<Tag, Guid>
    {
        private readonly Tag _tag;
        public Guid Id { get; set; }
        public string Title { get; set; }

        public TagFormModel(Tag tag)
        {
            Id = tag.Id;
            Title = tag.Title;
            _tag = tag;
        }

        public override Tag GetEntity()
        {
            _tag.Title = Title;
            return _tag;
        }
    }

    public class TagFormValidator : AbstractValidator<TagFormModel>
    {
        public TagFormValidator()
        {
            RuleFor(e => e.Title).NotEmpty().WithMessage("Укажите заголовок");
        }
    }
}
