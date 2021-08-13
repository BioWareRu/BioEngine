using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.IPB.Api;
using BioEngine.Core.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace BioEngine.Core.IPB.Users
{
    public class IPBCurrentUserProvider<TIpbModuleOptions> : ICurrentUserProvider
        where TIpbModuleOptions : IPBModuleOptions
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IPBApiClientFactory<TIpbModuleOptions> ipbApiClientFactory;
        private readonly IOptionsMonitor<IPBUsersModuleOptions> optionsMonitor;
        private IPBUsersModuleOptions usersModuleOptions => optionsMonitor.CurrentValue;

        public IPBCurrentUserProvider(IHttpContextAccessor httpContextAccessor,
            IPBApiClientFactory<TIpbModuleOptions> ipbApiClientFactory,
            IOptionsMonitor<IPBUsersModuleOptions> optionsMonitor)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.ipbApiClientFactory = ipbApiClientFactory;
            this.optionsMonitor = optionsMonitor;
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
                if (user.GetGroupIds().Contains(usersModuleOptions.AdminGroupId))
                {
                    user.SetAdmin();
                }
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
