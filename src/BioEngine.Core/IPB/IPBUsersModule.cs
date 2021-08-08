using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using BioEngine.Core.IPB.Api;
using BioEngine.Core.IPB.Users;
using BioEngine.Core.Users;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Sitko.Core.App;

namespace BioEngine.Core.IPB
{
    public class IPBUsersModule<TIPBModuleOptions> : BaseApplicationModule<IPBUsersModuleOptions>
        where TIPBModuleOptions : IPBModuleOptions
    {
        public override string OptionsKey => "Ipb:Users";

        public override void ConfigureServices(ApplicationContext context, IServiceCollection services,
            IPBUsersModuleOptions startupOptions)
        {
            base.ConfigureServices(context, services, startupOptions);
            services.AddScoped<IUserDataProvider, IPBUserDataProvider<TIPBModuleOptions>>();
            services.AddScoped<ICurrentUserProvider, IPBCurrentUserProvider<TIPBModuleOptions>>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(BioPolicies.User, builder => builder.RequireAuthenticatedUser());
                foreach (var (key, policy) in startupOptions.Policies)
                {
                    options.AddPolicy(key, policy);
                }
            });

            services.AddHttpClient();
            services.AddHttpContextAccessor();

            var signInScheme = "Cookies";
            var challengeScheme = "IPB";
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = signInScheme;
                    options.DefaultChallengeScheme = challengeScheme;
                })
                .AddCookie(signInScheme, options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromDays(30);
                    options.SlidingExpiration = true;
                }).AddOAuth(challengeScheme,
                    options =>
                    {
                        options.SignInScheme = signInScheme;
                        options.ClientId = startupOptions.ApiClientId;
                        options.ClientSecret = startupOptions.ApiClientSecret;
                        options.CallbackPath = new PathString(startupOptions.CallbackPath);
                        options.AuthorizationEndpoint = startupOptions.AuthorizationEndpoint;
                        options.TokenEndpoint = startupOptions.TokenEndpoint;
                        options.Scope.Add("profile");
                        options.SaveTokens = true;
                        options.Events = new OAuthEvents
                        {
                            OnCreatingTicket = async ticketContext =>
                            {
                                var factory = ticketContext.HttpContext.RequestServices
                                    .GetRequiredService<IPBApiClientFactory<TIPBModuleOptions>>();
                                var ipbApiClient = factory.GetClient(ticketContext.AccessToken);
                                var user = await ipbApiClient.GetUserAsync();

                                InsertClaims(user, ticketContext.Identity,
                                    options: startupOptions);
                            }
                        };
                    });
            if (!string.IsNullOrEmpty(startupOptions.DataProtectionPath))
            {
                services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(startupOptions.DataProtectionPath))
                    .SetApplicationName(context.Environment.ApplicationName)
                    .SetDefaultKeyLifetime(TimeSpan.FromDays(90));
            }
        }

        private void InsertClaims(User user, ClaimsIdentity identity, string? token = null,
            IPBUsersModuleOptions? options = null)
        {
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Name));
            identity.AddClaim(new Claim("photo", user.PhotoUrl));
            identity.AddClaim(new Claim(ClaimTypes.Webpage, user.ProfileUrl));
            if (!string.IsNullOrEmpty(token))
            {
                identity.AddClaim(new Claim("ipbToken", token));
            }

            var groups = user.GetGroupIds();
            identity.AddClaim(new Claim(ClaimTypes.PrimaryGroupSid, user.PrimaryGroup.Id.ToString()));
            foreach (var group in groups)
            {
                identity.AddClaim(new Claim(ClaimTypes.GroupSid, group.ToString()));
            }

            if (options != null)
            {
                if (groups.Contains(options.AdminGroupId))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, Roles.AdminRoleName));
                }

                if (groups.Intersect(options.AdditionalGroupIds).Any() || groups.Contains(options.AdminGroupId))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, Roles.SiteTeamRoleName));
                }
            }
        }
    }
}
