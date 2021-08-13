using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BioEngine.Core.IPB.Api
{
    public class IPBApiClientFactory<TOptions> where TOptions : IPBModuleOptions
    {
        private TOptions Options => optionsMonitor.CurrentValue;
        private readonly IOptionsMonitor<TOptions> optionsMonitor;
        private readonly ILoggerFactory loggerFactory;
        private readonly IHttpClientFactory httpClientFactory;

        public IPBApiClientFactory(IOptionsMonitor<TOptions> optionsMonitor, ILoggerFactory loggerFactory,
            IHttpClientFactory httpClientFactory)
        {
            this.optionsMonitor = optionsMonitor;
            this.loggerFactory = loggerFactory;
            this.httpClientFactory = httpClientFactory;
        }

        public IPBApiClient GetClient(string token) =>
            new(Options, token, null, loggerFactory.CreateLogger<IPBApiClient>(),
                httpClientFactory);

        public IPBApiClient GetReadOnlyClient()
        {
            if (string.IsNullOrEmpty(Options.ApiReadonlyKey))
            {
                throw new InvalidOperationException("Api readonly key don't configured");
            }

            return new IPBApiClient(Options, null, Options.ApiReadonlyKey,
                loggerFactory.CreateLogger<IPBApiClient>(), httpClientFactory);
        }

        public IPBApiClient GetPublishClient()
        {
            if (string.IsNullOrEmpty(Options.ApiPublishKey))
            {
                throw new InvalidOperationException("Api publish key don't configured");
            }

            return new IPBApiClient(Options, null, Options.ApiPublishKey, loggerFactory.CreateLogger<IPBApiClient>(),
                httpClientFactory);
        }
    }
}
