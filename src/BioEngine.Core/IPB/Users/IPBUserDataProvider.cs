using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.IPB.Api;
using BioEngine.Core.Users;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BioEngine.Core.IPB.Users
{
    public class IPBUserDataProvider<TIpbModuleOptions> : IUserDataProvider where TIpbModuleOptions : IPBModuleOptions
    {
        private readonly IPBApiClientFactory<TIpbModuleOptions> _clientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<IPBUserDataProvider<TIpbModuleOptions>> _logger;

        public IPBUserDataProvider(IPBApiClientFactory<TIpbModuleOptions> clientFactory, IMemoryCache memoryCache,
            ILogger<IPBUserDataProvider<TIpbModuleOptions>> logger)
        {
            _clientFactory = clientFactory;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        private static string GetCacheKey(string userId)
        {
            return $"ipbuserdata{userId}";
        }

        private List<User> GetFromCache(IEnumerable<string> userIds)
        {
            _logger.LogTrace("Get user data from cache");
            return userIds.Select(GetCacheKey).Select(key => _memoryCache.Get<User>(key))
                .Where(userData => userData != null).ToList();
        }

        public async Task<List<User>> GetDataAsync(string[] userIds)
        {
            var data = GetFromCache(userIds);
            var notFoundUserIds = userIds
                .Where(id => data.All(ud => ud.Id != id) && !string.IsNullOrEmpty(id) && id != "0").ToArray();
            if (notFoundUserIds.Length > 0)
            {
                _logger.LogTrace("Load users data from api");
                var tasks = notFoundUserIds.Select(id => GetApiClient().GetUserByIdAsync(id));
                var users = await Task.WhenAll(tasks);
                SetToCache(users);
                data.AddRange(users);
            }

            _logger.LogTrace("User data loaded");
            return data;
        }

        private void SetToCache(IEnumerable<User> userData)
        {
            _logger.LogTrace("Set user data to cache");
            foreach (var data in userData)
            {
                _memoryCache.Set(GetCacheKey(data.Id), data);
            }
        }

        private IPBApiClient? _apiClient;

        private IPBApiClient GetApiClient()
        {
            return _apiClient ??= _clientFactory.GetReadOnlyClient();
        }
    }
}
