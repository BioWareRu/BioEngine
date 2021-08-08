using System;
using BioEngine.Core.IPB.Api;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Sitko.Core.App;

namespace BioEngine.Core.IPB
{
    public abstract class IPBModule<TOptions> : BaseApplicationModule<TOptions> where TOptions : IPBModuleOptions, new()
    {
        public override void ConfigureServices(ApplicationContext context, IServiceCollection services,
            TOptions startupOptions)
        {
            base.ConfigureServices(context, services, startupOptions);
            services.AddSingleton<IPBApiClientFactory<TOptions>>();
            services.AddHttpClient();
        }
    }

    public abstract class IPBModuleOptions : BaseModuleOptions
    {
        public Uri? Url { get; set; } = null;
        public Uri ApiUrl => new Uri(Url!, "api");
        public string ApiReadonlyKey { get; set; } = "";
        public string ApiPublishKey { get; set; } = "";
    }

    public abstract class IPBModuleOptionsValidator<TOptions> : AbstractValidator<TOptions>
        where TOptions : IPBModuleOptions
    {
        public IPBModuleOptionsValidator()
        {
            RuleFor(o => o.Url).NotEmpty().WithMessage("IPB url is not set");
        }
    }
}
