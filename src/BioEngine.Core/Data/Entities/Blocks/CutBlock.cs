using System;
using FluentValidation;

namespace BioEngine.Core.Data.Entities.Blocks
{
    public record CutBlock : ContentBlock<CutBlockData>
    {
        public override string ToString()
        {
            return "";
        }
    }

    public record CutBlockData : ContentBlockData
    {
        public string ButtonText { get; set; } = "Читать дальше";
    }

    public class CutBlockValidator : AbstractValidator<CutBlock>
    {
        public CutBlockValidator()
        {
            RuleFor(d => d.Data.ButtonText).NotEmpty().WithMessage("Укажите текст кнопки");
        }
    }
}
