using System.Threading.Tasks;

namespace BioEngine.Core.Users
{
    public interface ICurrentUserProvider
    {
        Task<User?> GetCurrentUserAsync();
        Task<string?> GetAccessTokenAsync();
    }
}
