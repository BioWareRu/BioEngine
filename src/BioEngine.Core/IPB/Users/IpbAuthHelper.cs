using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using BioEngine.Core.IPB.Api;
using BioEngine.Core.Users;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sitko.Core.App;
using Sitko.Core.App.Json;

namespace BioEngine.Core.IPB.Users
{
    public static class IpbAuthHelper
    {
        public static void AddIpbOauthAuthentication(this IServiceCollection services,
            IPBUsersModuleOptions configuration, IHostEnvironment environment)
        {
            
        }

        
    }

    public class IPBUsersModuleOptions : BaseModuleOptions
    {
        public Dictionary<string, AuthorizationPolicy> Policies { get; } =
            new();

        public string ApiClientId { get; set; } = "";
        public string ApiClientSecret { get; set; } = "";
        public string CallbackPath { get; set; } = "";
        public string AuthorizationEndpoint { get; set; } = "";
        public string TokenEndpoint { get; set; } = "";
        public string DataProtectionPath { get; set; } = "";
        public bool DevMode { get; set; }
        public int AdminGroupId { get; set; }
        public int[] AdditionalGroupIds { get; set; } = new int[0];
    }
}
