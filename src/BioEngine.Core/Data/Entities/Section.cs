using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BioEngine.Core.Data.Entities.Abstractions;
using FluentValidation;
using Sitko.Blockly;
using Sitko.Core.Storage;

namespace BioEngine.Core.Data.Entities
{
    [Table("Sections")]
    public abstract class Section : BaseEntity, IContentEntity
    {
        public string Title { get; set; } = string.Empty;
        public abstract SectionType Type { get; set; }
        public virtual Guid? ParentId { get; set; }
        public bool IsPublished { get; set; }
        public DateTimeOffset? DatePublished { get; set; }
        public string Url { get; set; } = string.Empty;

        public List<ContentBlock> Blocks { get; set; } = new();
        public List<Post> Posts { get; set; } = new();
        public List<Site> Sites { get; set; } = new();

        public abstract void SetData(object data);
    }

    public abstract class Section<T> : Section where T : SectionData, new()
    {
        [Column(TypeName = "jsonb")] public virtual T Data { get; set; } = new();

        public override void SetData(object data)
        {
            if (data is T typedData)
            {
                Data = typedData;
            }
            else
            {
                throw new ArgumentException($"Wrong data type {data.GetType()} for entity {this}", nameof(data));
            }
        }
    }

    public abstract record SectionData
    {
        public StorageItem? HeaderPicture { get; set; }
        public string? Hashtag { get; set; }
    }

    public enum SectionType
    {
        None,
        Developer,
        Game,
        Topic
    }

    public abstract class BaseSectionValidator<TSection, TSectionData> : AbstractValidator<TSection>
        where TSection : Section<TSectionData>, new()
        where TSectionData : SectionData, new()
    {
        protected BaseSectionValidator()
        {
            RuleFor(e => e.Title).NotEmpty().WithMessage("Укажите заголовок").MaximumLength(1024).MinimumLength(5)
                .WithMessage("Заголовок должен быть от 5 до 1024 символов.");
            RuleFor(e => e.Url).NotEmpty().WithMessage("Укажите адрес");
            RuleFor(e => e.Sites)
                .NotEmpty()
                .WithMessage("Укажите сайты");
            // .OverridePropertyName(nameof(DeveloperFormModel.DummySiteId)); TODO: Show in form
            RuleFor(e => e.Blocks).NotEmpty();
        }
    }
}
