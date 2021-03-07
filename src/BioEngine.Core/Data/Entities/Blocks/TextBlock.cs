using System;
using FluentValidation;

namespace BioEngine.Core.Data.Entities.Blocks
{
    public record TextBlock : ContentBlock<TextBlockData>
    {
        public override string ToString()
        {
            return Data.Text;
        }
    }

    public record TextBlockData : ContentBlockData
    {
        public string Text { get; set; } = "";
    }

    public class TextBlockValidator : AbstractValidator<TextBlock>
    {
        public TextBlockValidator()
        {
            RuleFor(p => p.Data.Text).NotEmpty().WithMessage("Введите текст");
        }
    }
}
