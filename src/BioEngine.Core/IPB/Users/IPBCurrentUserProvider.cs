using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BioEngine.Core.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace BioEngine.Core.IPB.Users
{
    public class IPBCurrentUserProvider : ICurrentUserProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IPBCurrentUserProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public User? CurrentUser
        {
            get
            {
                if (_httpContextAccessor.HttpContext == null)
                {
                    return null;
                }

                var user = new User
                {
                    Id =
                        _httpContextAccessor.HttpContext.User.Claims
                            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                    Name =
                        _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)
                            ?.Value,
                    PhotoUrl =
                        _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "photo")?.Value,
                    ProfileUrl = _httpContextAccessor.HttpContext.User.Claims
                        .FirstOrDefault(c => c.Type == ClaimTypes.Webpage)?.Value
                };
                if (!string.IsNullOrEmpty(_httpContextAccessor.HttpContext.User.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.PrimaryGroupSid)?.Value))
                {
                    if (int.TryParse(_httpContextAccessor.HttpContext.User.Claims
                        .FirstOrDefault(c => c.Type == ClaimTypes.PrimaryGroupSid)?.Value, out var groupId))
                    {
                        user.PrimaryGroup = new Group {Id = groupId};
                    }
                }

                var secondaryGroupIds = _httpContextAccessor.HttpContext.User.Claims
                    .Where(c => c.Type == ClaimTypes.GroupSid).Select(c => c.Value).ToList();
                if (secondaryGroupIds.Any())
                {
                    var groups = new List<Group>();
                    foreach (var secondaryGroupId in secondaryGroupIds)
                    {
                        if (int.TryParse(secondaryGroupId, out var groupId))
                        {
                            groups.Add(new Group {Id = groupId});
                        }
                    }

                    user.SecondaryGroups = groups.ToArray();
                }

                return user;
            }
        }


        public Task<string?> GetAccessTokenAsync()
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                return _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            }
            string? token = null;
            return Task.FromResult(token);
        }
    }
}
