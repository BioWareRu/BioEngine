using System;
using FluentValidation;

namespace BioEngine.Core.Data.Entities.Blocks
{
    public record IframeBlock : ContentBlock<IframeBlockData>
    {
        public override string ToString()
        {
            return $"Frame: {Data.Src}";
        }
    }

    public record IframeBlockData : ContentBlockData
    {
        public string Src { get; set; } = "";
        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;
    }

    public class IframeBlockValidator : AbstractValidator<IframeBlock>
    {
        public IframeBlockValidator()
        {
            RuleFor(p => p.Data.Src).NotEmpty().WithMessage("Укажите ссылку")
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)).WithMessage("Значение должно быть ссылкой");
        }
    }
}
