using System;
using System.Collections.Generic;
using FluentValidation;

namespace BioEngine.Core.Data.Entities
{
    public class Site : BaseEntity
    {
        public string Url { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public List<Post> Posts { get; set; }
        public List<Page> Pages { get; set; }
        public List<Section> Sections { get; set; }
    }

    public class SiteValidator : AbstractValidator<Site>
    {
        public SiteValidator()
        {
            RuleFor(e => e.Title).NotEmpty().WithMessage("Укажите заголовок")
                .MaximumLength(1024).MinimumLength(5)
                .WithMessage("Заголовок должен быть от 5 до 1024 символов.");
            RuleFor(e => e.Url).NotEmpty().WithMessage("Укажите адрес")
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .WithMessage("Значение должно быть ссылкой");
        }
    }
}
