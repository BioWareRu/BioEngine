using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using BioEngine.Core.Data.Entities.Abstractions;
using BioEngine.Core.Users;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Sitko.Blockly;

namespace BioEngine.Core.Data.Entities
{
    [Table("Posts")]
    public class Post : BaseEntity, IContentItem
    {
        public string AuthorId { get; set; }
        [NotMapped] public User Author { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public bool IsPublished { get; set; }
        public DateTimeOffset? DatePublished { get; set; }

        public List<Section> Sections { get; set; } = new();
        public List<Tag> Tags { get; set; } = new();

        public List<ContentBlock> Blocks { get; set; } = new();
    }

    [UsedImplicitly]
    public class PostValidator : AbstractValidator<Post>
    {
        public PostValidator(BioDbContext dbContext)
        {
            RuleFor(e => e.Title).NotEmpty().WithMessage("Укажите заголовок").MaximumLength(1024).MinimumLength(5)
                .WithMessage("Заголовок должен быть от 5 до 1024 символов.");
            RuleFor(e => e.Url).NotEmpty().WithMessage("Укажите адрес");
            RuleFor(e => e.Sections)
                .NotEmpty()
                .WithMessage("Укажите разделы");
            // .OverridePropertyName(nameof(PostFormModel.DummySectionId)); TODO: Show in form

            RuleFor(e => e.Url).CustomAsync(async (url, context, cancellationToken) =>
            {
                if (context.InstanceToValidate is not null && !string.IsNullOrEmpty(url))
                {
                    var count = await dbContext.Set<Post>()
                        .Where(p => p.Url == url && p.Id != context.InstanceToValidate.Id)
                        .CountAsync(cancellationToken);
                    if (count > 0)
                    {
                        context.AddFailure(
                            $"Url {url} already taken");
                    }
                }
            });
            RuleFor(e => e.Blocks).NotEmpty();
        }
    }
}
