using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BioEngine.Core.Data.Entities;
using BioEngine.Core.Users;
using Sitko.Core.Repository.EntityFrameworkCore;

namespace BioEngine.Core.Data.Repositories
{
    public class PostsRepository : ContentItemRepository<Post>
    {
        private readonly IUserDataProvider userDataProvider;


        public PostsRepository(EFRepositoryContext<Post, Guid, BioDbContext> repositoryContext,
            SectionsRepository sectionsRepository,
            IUserDataProvider userDataProvider) : base(repositoryContext, sectionsRepository) =>
            this.userDataProvider = userDataProvider;

        protected override async Task AfterLoadAsync(Post[] entities,
            CancellationToken cancellationToken = default)
        {
            await base.AfterLoadAsync(entities, cancellationToken);

            var userIds = entities.Select(e => e.AuthorId).Distinct().ToArray();
            var users = await userDataProvider.GetDataAsync(userIds);

            foreach (var entity in entities)
            {
                entity.Author = users.First(d => d.Id.Equals(entity.AuthorId, StringComparison.Ordinal));
            }
        }


        // protected override async Task<bool> AfterSaveAsync(IEnumerable<RepositoryRecord<Post, Guid>> items,
        //     CancellationToken cancellationToken = default)
        // {
        //     var user = _currentUserProvider.CurrentUser;
        //     foreach (var record in items)
        //     {
        //         var version = new PostVersion {Id = Guid.NewGuid(), ContentId = record.Item.Id};
        //         version.SetContent(record.Item);
        //
        //         if (user != null)
        //         {
        //             version.ChangeAuthorId = user.Id;
        //         }
        //
        //         await Set<PostVersion>().AddAsync(version, cancellationToken);
        //         await DoSaveAsync(cancellationToken);
        //     }
        //
        //
        //     return await base.AfterSaveAsync(items, cancellationToken);
        // }

        // public async Task<List<PostVersion>> GetVersionsAsync(Guid itemId)
        // {
        //     return await Set<PostVersion>().Where(v => v.ContentId == itemId).ToListAsync();
        // }
        //
        // public async Task<PostVersion> GetVersionAsync(Guid itemId, Guid versionId)
        // {
        //     return await Set<PostVersion>().Where(v => v.ContentId == itemId && v.Id == versionId)
        //         .FirstOrDefaultAsync();
        // }
    }
}
