using System;
using FluentValidation;
using Sitko.Core.Storage;

namespace BioEngine.Core.Data.Entities.Blocks
{
    public record QuoteBlock : ContentBlock<QuoteBlockData>
    {
        public override string ToString()
        {
            return $"{Data.Author}: {Data.Text} ({Data.Link})";
        }
    }

    public record QuoteBlockData : ContentBlockData
    {
        public string Text { get; set; } = string.Empty;
        public string? Author { get; set; }
        public string? Link { get; set; }
        public StorageItem? Picture { get; set; }
    }
    
    public class QuoteBlockValidator : AbstractValidator<QuoteBlock>
    {
        public QuoteBlockValidator()
        {
            RuleFor(d => d.Data.Text).NotEmpty().WithMessage("Введите текст цитаты");
            RuleFor(p => p.Data.Link).Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .When(x => !string.IsNullOrEmpty(x.Data.Link)).WithMessage("Значение должно быть ссылкой");
        }
    }
}
