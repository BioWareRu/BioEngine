using System.Threading.Tasks;

namespace BioEngine.Core.Users
{
    public interface ICurrentUserProvider
    {
        User? CurrentUser { get; }
        Task<string?> GetAccessTokenAsync();
    }
}
