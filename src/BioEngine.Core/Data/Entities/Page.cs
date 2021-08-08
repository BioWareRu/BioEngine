using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BioEngine.Core.Data.Entities.Abstractions;
using FluentValidation;
using Sitko.Blockly;

namespace BioEngine.Core.Data.Entities
{
    [Table("Pages")]
    public class Page : BaseEntity, IContentEntity
    {
        public List<Site> Sites { get; set; } = new();
        public string Url { get; set; }
        public List<ContentBlock> Blocks { get; set; } = new();
        public string Title { get; set; }
        public bool IsPublished { get; set; }
        public DateTimeOffset? DatePublished { get; set; }
    }

    public class PageFormValidator : AbstractValidator<Page>
    {
        public PageFormValidator()
        {
            RuleFor(e => e.Title).NotEmpty().WithMessage("Укажите заголовок").MaximumLength(1024).MinimumLength(5)
                .WithMessage("Заголовок должен быть от 5 до 1024 символов.");
            RuleFor(e => e.Url).NotEmpty().WithMessage("Укажите адрес");
            RuleFor(e => e.Sites)
                .NotEmpty()
                .WithMessage("Укажите сайты");
            // .OverridePropertyName(nameof(PageFormModel.DummySiteId)); TODO: Display in form
            RuleFor(e => e.Blocks).NotEmpty();
        }
    }
}
