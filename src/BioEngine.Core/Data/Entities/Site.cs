using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BioEngine.Core.Data.Entities
{
    public class Site : BaseEntity
    {
        public string Url { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public bool IsMain { get; set; }

        public List<Page> Pages { get; set; } = new();
        public List<Section> Sections { get; set; } = new();
        public int ForumId { get; set; }
    }

    public class SiteValidator : AbstractValidator<Site>
    {
        public SiteValidator(BioDbContext dbContext)
        {
            RuleFor(e => e.Title).NotEmpty().WithMessage("Укажите заголовок")
                .MaximumLength(1024).MinimumLength(5)
                .WithMessage("Заголовок должен быть от 5 до 1024 символов.");
            RuleFor(e => e.Url).NotEmpty().WithMessage("Укажите адрес")
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .WithMessage("Значение должно быть ссылкой");
            RuleFor(e => e.ForumId).GreaterThan(0).WithMessage("Выберите раздел на форуме");
            RuleFor(e => e.Url).CustomAsync(async (url, context, cancellationToken) =>
            {
                if (context.InstanceToValidate is not null && !string.IsNullOrEmpty(url))
                {
                    var count = await dbContext.Set<Site>()
                        .Where(p => p.Url == url && p.Id != context.InstanceToValidate.Id)
                        .CountAsync(cancellationToken);
                    if (count > 0)
                    {
                        context.AddFailure(
                            $"Адрес {url} уже используется");
                    }
                }
            });
            RuleFor(e => e.Title).CustomAsync(async (title, context, cancellationToken) =>
            {
                if (context.InstanceToValidate is not null && !string.IsNullOrEmpty(title))
                {
                    var count = await dbContext.Set<Site>()
                        .Where(p => p.Title == title && p.Id != context.InstanceToValidate.Id)
                        .CountAsync(cancellationToken);
                    if (count > 0)
                    {
                        context.AddFailure(
                            $"Название {title} уже используется");
                    }
                }
            });
            RuleFor(e => e.IsMain).CustomAsync(async (isMain, context, cancellationToken) =>
            {
                if (context.InstanceToValidate is not null && isMain)
                {
                    var count = await dbContext.Set<Site>()
                        .Where(p => p.IsMain == isMain && p.Id != context.InstanceToValidate.Id)
                        .CountAsync(cancellationToken);
                    if (count > 0)
                    {
                        context.AddFailure(
                            "Главный сайт уже выбран");
                    }
                }
            });
        }
    }
}
