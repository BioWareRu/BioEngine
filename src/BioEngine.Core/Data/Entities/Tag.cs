using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;

namespace BioEngine.Core.Data.Entities
{
    [Table("Tags")]
    public class Tag : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        
        public List<Post> Posts { get; set; }
    }

    public class TagValidator : AbstractValidator<Tag>
    {
        public TagValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
        }
    }
}
