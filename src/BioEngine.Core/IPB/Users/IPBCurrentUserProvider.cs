using System.Threading.Tasks;
using BioEngine.Core.IPB.Api;
using BioEngine.Core.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace BioEngine.Core.IPB.Users
{
    public class IPBCurrentUserProvider<TIpbModuleOptions> : ICurrentUserProvider
        where TIpbModuleOptions : IPBModuleOptions
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IPBApiClientFactory<TIpbModuleOptions> ipbApiClientFactory;

        public IPBCurrentUserProvider(IHttpContextAccessor httpContextAccessor,
            IPBApiClientFactory<TIpbModuleOptions> ipbApiClientFactory)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.ipbApiClientFactory = ipbApiClientFactory;
        }

        private User? user;


        public async Task<User?> GetCurrentUserAsync()
        {
            if (httpContextAccessor.HttpContext == null)
            {
                return null;
            }

            if (httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            if (user is null)
            {
                var client = ipbApiClientFactory.GetClient(await GetAccessTokenAsync());
                user = await client.GetUserAsync();
            }

            return user;
        }

        public Task<string?> GetAccessTokenAsync()
        {
            if (httpContextAccessor.HttpContext != null)
            {
                return httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            }

            string? token = null;
            return Task.FromResult(token);
        }
    }
}
