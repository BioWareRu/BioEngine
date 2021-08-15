using System;
using BioEngine.Core.Data.Entities.Abstractions;
using Sitko.Core.Repository.EntityFrameworkCore;

namespace BioEngine.Core.Data.Repositories
{
    public abstract class SectionEntityRepository<TEntity> : PublishableEntityRepository<TEntity>
        where TEntity : class, ISectionEntity, IContentEntity
    {
        protected SectionEntityRepository(EFRepositoryContext<TEntity, Guid, BioDbContext> repositoryContext) :
            base(repositoryContext)
        {
        }

        // protected override async Task<bool> BeforeSaveAsync(TEntity item,
        //     (bool isValid, IList<ValidationFailure> errors) validationResult, bool isNew,
        //     CancellationToken cancellationToken = default)
        // {
        //     var result = await base.BeforeSaveAsync(item, validationResult, isNew, cancellationToken);
        //     if (result)
        //     {
        //         await ExecuteDbContextOperationAsync(async context =>
        //         {
        //             //await context.Entry(item).Collection(p => p.Sites).LoadAsync(cancellationToken);
        //             foreach (var section in context.Sections)
        //             {
        //                 await context.Entry(section).Collection(p => p.Sites).LoadAsync(cancellationToken);
        //             }
        //             return true;
        //         });
        //         item.Sites = item.Sections.SelectMany(s => s.Sites).Distinct().ToList();
        //     }
        //
        //     return result;
        // }
    }
}
