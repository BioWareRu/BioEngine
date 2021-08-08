using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BioEngine.Core.Users;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BioEngine.Core.IPB.Api
{
    public class IPBApiClientFactory<TOptions> where TOptions : IPBModuleOptions
    {
        private TOptions Options => _optionsMonitor.CurrentValue;
        private readonly IOptionsMonitor<TOptions> _optionsMonitor;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IHttpClientFactory _httpClientFactory;

        public IPBApiClientFactory(IOptionsMonitor<TOptions> optionsMonitor, ILoggerFactory loggerFactory,
            IHttpClientFactory httpClientFactory)
        {
            _optionsMonitor = optionsMonitor;
            _loggerFactory = loggerFactory;
            _httpClientFactory = httpClientFactory;
        }

        public IPBApiClient GetClient(string token)
        {
            return new IPBApiClient(Options, token, null, _loggerFactory.CreateLogger<IPBApiClient>(),
                _httpClientFactory);
        }

        public IPBApiClient GetReadOnlyClient()
        {
            if (string.IsNullOrEmpty(Options.ApiReadonlyKey))
            {
                throw new Exception("Api readonly key don't configured");
            }

            return new IPBApiClient(Options, null, Options.ApiReadonlyKey,
                _loggerFactory.CreateLogger<IPBApiClient>(), _httpClientFactory);
        }

        public IPBApiClient GetPublishClient()
        {
            if (string.IsNullOrEmpty(Options.ApiPublishKey))
            {
                throw new Exception("Api publish key don't configured");
            }

            return new IPBApiClient(Options, null, Options.ApiPublishKey, _loggerFactory.CreateLogger<IPBApiClient>(),
                _httpClientFactory);
        }
    }

    public class IPBApiClient
    {
        private readonly IPBModuleOptions _options;
        private readonly string? _token;
        private readonly string? _apiKey;
        private readonly ILogger<IPBApiClient> _logger;
        private readonly FlurlClient _flurlClient;

        public IPBApiClient(IPBModuleOptions options, string? token, string? apiKey, ILogger<IPBApiClient> logger,
            IHttpClientFactory httpClientFactory)
        {
            _options = options;
            _token = token;
            _apiKey = apiKey;
            _logger = logger;
            _flurlClient = new FlurlClient(httpClientFactory.CreateClient());
        }

        public Task<User> GetUserAsync()
        {
            return GetAsync<User>("core/me");
        }

        private IFlurlRequest GetRequest(string url)
        {
            var requestUrl = new FlurlRequest($"{_options.ApiUrl}/{url}").WithClient(_flurlClient);
            if (!string.IsNullOrEmpty(_token))
            {
                requestUrl.WithOAuthBearerToken(_token);
            }
            else
            {
                if (!string.IsNullOrEmpty(_apiKey))
                {
                    requestUrl.SetQueryParam("key", _apiKey);
                }
                else
                {
                    throw new Exception("No token and no key for ipb client");
                }
            }

            return requestUrl;
        }


        private async Task<T> GetAsync<T>(string url)
        {
            try
            {
                return await GetRequest(url).GetJsonAsync<T>();
            }
            catch (FlurlHttpException ex)
            {
                _logger.LogError(ex, "Error in request to IPB: {errorText}. Response: {response}", ex.ToString(),
                    await ex.GetResponseStringAsync());
                throw;
            }
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest item)
        {
            try
            {
                if (item != null)
                {
                    var data = item.ToKeyValue();
                    var response = await GetRequest(url).PostUrlEncodedAsync(data);
                    var json = await response.GetStringAsync();
                    if (response.ResponseMessage.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<TResponse>(json);
                    }

                    throw new IPBApiException(response.ResponseMessage.StatusCode,
                        JsonConvert.DeserializeObject<IPBApiError>(json));
                }

                throw new IPBApiException(HttpStatusCode.BadRequest, new IPBApiError {ErrorMessage = "Empty request"});
            }
            catch (FlurlHttpException ex)
            {
                _logger.LogError(ex, "Error in request to IPB: {errorText}. Response: {response}", ex.ToString(),
                    await ex.GetResponseStringAsync());
                throw;
            }
        }

        // public Task<Response<Forum>> GetForumsAsync(int page = 1, int perPage = 25)
        // {
        //     return GetAsync<Response<Forum>>($"forums/forums?page={page.ToString()}&perPage={perPage.ToString()}");
        // }

        public Task<User> GetUserByIdAsync(string id)
        {
            return GetAsync<User>($"core/members/{id}");
        }

        // public Task<ForumTopic> GetTopicAsync(int topicId)
        // {
        //     return GetAsync<ForumTopic>($"forums/topics/{topicId.ToString()}");
        // }

        // public Task<Response<ForumPost>> GetForumsPostsAsync(int[] forumIds, string? orderBy = null,
        //     bool orderDescending = false, int page = 1, int perPage = 100)
        // {
        //     if (forumIds == null || forumIds.Length == 0)
        //     {
        //         throw new ArgumentException(nameof(forumIds));
        //     }
        //
        //     var url = $"forums/posts?page={page.ToString()}&perPage={perPage.ToString()}";
        //     if (!string.IsNullOrEmpty(orderBy))
        //     {
        //         url += $"&sortBy={orderBy}";
        //     }
        //
        //     if (orderDescending)
        //     {
        //         url += "&sortDir=desc";
        //     }
        //
        //     url += $"&forums={string.Join(',', forumIds)}";
        //
        //     return GetAsync<Response<ForumPost>>(url);
        // }
        //
        // public Task<Response<ForumPost>> GetTopicPostsAsync(int topicId, int page = 1, int perPage = 25)
        // {
        //     return GetAsync<Response<ForumPost>>(
        //         $"forums/topics/{topicId.ToString()}/posts?page={page.ToString()}&perPage={perPage.ToString()}");
        // }
    }

    public static class IPBApiClientHelper
    {
        public static IDictionary<string, string>? ToKeyValue(this object metaToken)
        {
            if (!(metaToken is JToken token))
            {
                return ToKeyValue(JObject.FromObject(metaToken));
            }

            if (token.HasValues)
            {
                var contentData = new Dictionary<string, string>();
                foreach (var child in token.Children().ToList())
                {
                    var childContent = child.ToKeyValue();
                    if (childContent != null)
                    {
                        contentData = contentData.Concat(childContent)
                            .ToDictionary(k => k.Key.ToLowerInvariant(), v => v.Value);
                    }
                }

                return contentData;
            }

            var jValue = token as JValue;
            if (jValue?.Value == null)
            {
                return null;
            }

            var value = jValue.Type == JTokenType.Date
                ? jValue.ToString("o", CultureInfo.InvariantCulture)
                : jValue.ToString(CultureInfo.InvariantCulture);

            return new Dictionary<string, string> {{token.Path.ToLowerInvariant(), value}};
        }
    }
}
