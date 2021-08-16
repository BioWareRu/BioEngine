using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BioEngine.Core.IPB.Models;
using BioEngine.Core.Users;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BioEngine.Core.IPB.Api
{
    public class IPBApiClient : IDisposable
    {
        private readonly IPBModuleOptions options;
        private readonly string? token;
        private readonly string? apiKey;
        private readonly ILogger<IPBApiClient> logger;
        private readonly FlurlClient flurlClient;

        public IPBApiClient(IPBModuleOptions options, string? token, string? apiKey, ILogger<IPBApiClient> logger,
            IHttpClientFactory httpClientFactory)
        {
            this.options = options;
            this.token = token;
            this.apiKey = apiKey;
            this.logger = logger;
            flurlClient = new FlurlClient(httpClientFactory.CreateClient());
        }

        public Task<User> GetUserAsync() => GetAsync<User>("core/me");

        private IFlurlRequest GetRequest(string url)
        {
            var requestUrl = new FlurlRequest($"{options.ApiUrl}/{url}").WithClient(flurlClient);
            if (!string.IsNullOrEmpty(token))
            {
                requestUrl.WithOAuthBearerToken(token);
            }
            else
            {
                if (!string.IsNullOrEmpty(apiKey))
                {
                    requestUrl.SetQueryParam("key", apiKey);
                }
                else
                {
                    throw new InvalidOperationException("No token and no key for ipb client");
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
                logger.LogError(ex, "Error in request to IPB: {ErrorText}. Response: {Response}", ex.ToString(),
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

                throw new IPBApiException(HttpStatusCode.BadRequest,
                    new IPBApiError { ErrorMessage = "Empty request" });
            }
            catch (FlurlHttpException ex)
            {
                logger.LogError(ex, "Error in request to IPB: {ErrorText}. Response: {Response}", ex.ToString(),
                    await ex.GetResponseStringAsync());
                throw;
            }
        }

        public async Task<List<Forum>> GetForumsAsync()
        {
            var page = 1;
            var perPage = 100;
            var forums = new List<Forum>();
            while (true)
            {
                var response = await GetAsync<Response<Forum>>($"forums/forums?page={page.ToString(CultureInfo.CurrentCulture)}&perPage={perPage.ToString(CultureInfo.CurrentCulture)}");
                forums.AddRange(response.Results);
                if (response.TotalResults == forums.Count)
                {
                    break;
                }

                page++;
            }

            return forums;
        }

        public Task<User> GetUserByIdAsync(string id) => GetAsync<User>($"core/members/{id}");

        public void Dispose()
        {
            flurlClient.Dispose();
            GC.SuppressFinalize(this);
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

            return new Dictionary<string, string> { { token.Path.ToLowerInvariant(), value } };
        }
    }
}
