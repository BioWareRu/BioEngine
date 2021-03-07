using System;
using FluentValidation;
using Sitko.Core.Storage;

namespace BioEngine.Core.Data.Entities.Blocks
{
    public record PictureBlock : ContentBlock<PictureBlockData>
    {
        public override string ToString()
        {
            return Data.Picture is null ? "Картинка не выбрана" : $"Картинка: {Data.Picture.FileName}";
        }
    }

    public record PictureBlockData : ContentBlockData
    {
        public StorageItem? Picture { get; set; }
        public string? Url { get; set; }
    }

    public class PictureBlockValidator : AbstractValidator<PictureBlock>
    {
        public PictureBlockValidator()
        {
            RuleFor(p => p.Data.Picture).NotNull().WithMessage("Выберите картину");
            RuleFor(p => p.Data.Url).Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .When(x => !string.IsNullOrEmpty(x.Data.Url)).WithMessage("Значение должно быть ссылкой");
        }
    }
}
