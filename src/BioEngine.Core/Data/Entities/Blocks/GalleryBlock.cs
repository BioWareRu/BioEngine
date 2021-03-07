using System.Linq;
using FluentValidation;
using Sitko.Core.Storage;

namespace BioEngine.Core.Data.Entities.Blocks
{
    public record GalleryBlock : ContentBlock<GalleryBlockData>
    {
        public override string ToString()
        {
            return $"Галерея: {string.Join(", ", Data.Pictures.Select(p => p.FileName))}";
        }
    }

    public record GalleryBlockData : ContentBlockData
    {
        public ValueCollection<StorageItem> Pictures { get; set; } = new();
    }

    public class GalleryBlockValidator : AbstractValidator<GalleryBlock>
    {
        public GalleryBlockValidator()
        {
            RuleFor(d => d.Data.Pictures).NotEmpty().WithMessage("Выберите картинки");
        }
    }
}
