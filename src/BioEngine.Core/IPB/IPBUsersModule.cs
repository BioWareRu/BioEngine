using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using BioEngine.Core.IPB.Api;
using BioEngine.Core.IPB.Users;
using BioEngine.Core.Users;
using Flurl.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
                    options.Events = new CookieAuthenticationEvents
                    {
                        OnValidatePrincipal = async cookieValidatePrincipalContext =>
                        {
                            if (cookieValidatePrincipalContext.Principal?.Identity?.IsAuthenticated == true)
                            {
                                var logger = cookieValidatePrincipalContext.HttpContext.RequestServices
                                    .GetRequiredService<ILogger<IPBUsersModule<TIPBModuleOptions>>>();
                                //get the user's tokens
                                var tokens = cookieValidatePrincipalContext.Properties.GetTokens().ToList();
                                var refreshToken = tokens.FirstOrDefault(t => t.Name == "refresh_token");
                                var accessToken = tokens.FirstOrDefault(t => t.Name == "access_token");
                                var exp = tokens.FirstOrDefault(t => t.Name == "expires_at");
                                if (accessToken is null)
                                {
                                    logger.LogError("Access token is null");
                                    cookieValidatePrincipalContext.RejectPrincipal();
                                    return;
                                }

                                if (exp is not null)
                                {
                                    var expires = DateTime.Parse(exp.Value, CultureInfo.InvariantCulture);
                                    //check to see if the token has expired
                                    if (expires < DateTime.Now)
                                    {
                                        if (refreshToken is null)
                                        {
                                            logger.LogError("Refresh token is null");
                                            cookieValidatePrincipalContext.RejectPrincipal();
                                            return;
                                        }

                                        //token is expired, let's attempt to renew
                                        try
                                        {
                                            var request = new FlurlRequest(startupOptions.TokenEndpoint)
                                                .SetQueryParam("grant_type", "refresh_token")
                                                .SetQueryParam("refresh_token", refreshToken.Value)
                                                .WithAutoRedirect(true);
                                            var result = await request.PostAsync(new FormUrlEncodedContent(new[]
                                            {
                                                new KeyValuePair<string, string>("client_id",
                                                    startupOptions.ApiClientId),
                                                new KeyValuePair<string, string>("client_secret",
                                                    startupOptions.ApiClientSecret)
                                            }));
                                            var response = await result.GetJsonAsync<TokenResponse>();
                                            accessToken.Value = response.AccessToken;
                                            refreshToken.Value = response.RefreshToken;
                                            var newExpires = DateTime.UtcNow + TimeSpan.FromSeconds(response.ExpiresIn);
                                            exp.Value = newExpires.ToString("o", CultureInfo.InvariantCulture);
                                            cookieValidatePrincipalContext.Properties.StoreTokens(tokens);
                                            cookieValidatePrincipalContext.ShouldRenew = true;
                                            logger.LogInformation("Token refreshed");
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.LogError(ex, "Error refreshing token: {ErrorText}", ex.ToString());
                                            cookieValidatePrincipalContext.RejectPrincipal();
                                        }
                                    }
                                }
                                else
                                {
                                    logger.LogError("Expiration time is null");
                                    cookieValidatePrincipalContext.RejectPrincipal();
                                }
                            }
                        }
                    };
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
                                if (ticketContext.Identity?.IsAuthenticated == true &&
                                    !string.IsNullOrEmpty(ticketContext.AccessToken))
                                {
                                    var factory = ticketContext.HttpContext.RequestServices
                                        .GetRequiredService<IPBApiClientFactory<TIPBModuleOptions>>();
                                    var ipbApiClient = factory.GetClient(ticketContext.AccessToken);
                                    var user = await ipbApiClient.GetUserAsync();

                                    InsertClaims(user, ticketContext.Identity,
                                        options: startupOptions);
                                }
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

        private static void InsertClaims(User user, ClaimsIdentity identity, string? token = null,
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
            identity.AddClaim(new Claim(ClaimTypes.PrimaryGroupSid,
                user.PrimaryGroup.Id.ToString(CultureInfo.InvariantCulture)));
            foreach (var group in groups)
            {
                identity.AddClaim(new Claim(ClaimTypes.GroupSid, group.ToString(CultureInfo.InvariantCulture)));
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

    public class TokenResponse
    {
        [JsonProperty("access_token")] public string AccessToken { get; set; } = "";
        [JsonProperty("refresh_token")] public string RefreshToken { get; set; } = "";
        [JsonProperty("expires_in")] public int ExpiresIn { get; set; }
    }
}
