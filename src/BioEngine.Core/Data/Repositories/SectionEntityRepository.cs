using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BioEngine.Core.Data.Entities.Abstractions;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sitko.Core.Repository.EntityFrameworkCore;

namespace BioEngine.Core.Data.Repositories
{
    public abstract class SectionEntityRepository<TEntity> : PublishableEntityRepository<TEntity>
        where TEntity : class, ISectionEntity, IContentEntity
    {
        protected readonly SectionsRepository SectionsRepository;


        protected SectionEntityRepository(EFRepositoryContext<TEntity, Guid, BioDbContext> repositoryContext, SectionsRepository sectionsRepository) :
            base(repositoryContext)
        {
            SectionsRepository = sectionsRepository;
        }

        protected override async Task<bool> BeforeValidateAsync(TEntity item,
            (bool isValid, IList<ValidationFailure> errors) validationResult,
            bool isNew, CancellationToken cancellationToken = default)
        {
            var result = await base.BeforeValidateAsync(item, validationResult, isNew, cancellationToken);

            if (!result)
                return false;

            if (item.Sections.Any())
            {
                var sections = item.Sections;

                if (sections.Any())
                {
                    item.Sites = sections.SelectMany(s => s.Sites).Distinct().ToList();
                    if (item.Sites.Any())
                    {
                        return true;
                    }

                    validationResult.errors.Add(new ValidationFailure(nameof(ISiteEntity.Sites),
                        "Не найдены сайты"));
                }
                else
                {
                    validationResult.errors.Add(
                        new ValidationFailure(nameof(ISiteEntity.Sites), "Не найдены разделы"));
                }
            }
            else
            {
                validationResult.errors.Add(new ValidationFailure(nameof(ISectionEntity.Sections),
                    "Не указаны разделы"));
            }

            return false;
        }

        protected override IQueryable<TEntity> AddIncludes(IQueryable<TEntity> query)
        {
            return base
                .AddIncludes(query)
                .Include(p => p.Sections).ThenInclude(s => s.Sites)
                .Include(p => p.Tags);
        }
    }
}
