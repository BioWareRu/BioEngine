using System;
using BioEngine.Core.Extensions;
using FluentValidation;
using Sitko.Core.Storage;

namespace BioEngine.Core.Data.Entities.Blocks
{
    public record FileBlock : ContentBlock<FileBlockData>
    {
        public override string ToString()
        {
            return Data.File is null ? "Файл не выбран" : $"Файл: {Data.File.FileName}";
        }
    }

    public record FileBlockData : ContentBlockData
    {
        public StorageItem? File { get; set; }
    }
    
    public class FileBlockValidator : AbstractValidator<FileBlock>
    {
        public FileBlockValidator()
        {
            RuleFor(d => d.Data.File).NotNull().WithMessage("Выберите файл");
        }
    }
}
