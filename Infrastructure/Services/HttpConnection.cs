using Application.ConfigSettings;
using Application.InfraInterfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class HttpConnection : IHttpConnection
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HttpConnection> _logger;
        private DevApiSettings DevApiConfig { get; }
        public HttpConnection(IHttpClientFactory httpClientFactory, ILogger<HttpConnection> logger, IOptions<DevApiSettings> devApiConfig)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            DevApiConfig = devApiConfig.Value;
        }

        public async Task<T> DevAPIRequest<T>(HttpRequestMessage request, HttpContent content, string client) where T : new()
        {
            var httpClient = _httpClientFactory.CreateClient(client);

            var utcdate = DateTime.UtcNow;
            var timeStamp = utcdate.ToString("yyyy-MM-ddTHH:mm:ss.fff");
            var xtoken = GetXTokenHeader(utcdate, DevApiConfig.ProductKeys.ClientId, DevApiConfig.ProductKeys.Password);
            request.Headers.TryAddWithoutValidation("client_id", DevApiConfig.ProductKeys.ClientId);
            request.Headers.TryAddWithoutValidation("UTCTimestamp", timeStamp);
            request.Headers.TryAddWithoutValidation("x-token", xtoken);
            request.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", DevApiConfig.ProductKeys.SubscriptionKey);
            request.Content = content;
            var response = await httpClient.SendAsync(request);
            string apiResponse = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"API Execute Response : {apiResponse}");
            try
            {
                var deserialisedResponse = JsonConvert.DeserializeObject<T>(apiResponse);
                return deserialisedResponse;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error occurred while deserilizing response : {ex}");
                const string message = "Error retrieving response.  Check inner details for more info.";
                var exception = new Exception(message, ex);
                throw exception;
            }
        }

        public async Task<T> WebRequest<T>(string url, object request, string requestType, Dictionary<string, string> headers = null, string authUserName = null, string authPword = null) where T : new()
        {
            T result = new T();

            Method method = requestType.ToLower() == "post" ? Method.Post : Method.Get;
            var client = new RestClient(url);
            var restRequest = new RestRequest(url, method);

            if (method == Method.Post)
            {
                restRequest.RequestFormat = DataFormat.Json;
                restRequest.AddJsonBody(request);
            }

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    restRequest.AddHeader(item.Key, item.Value);
                }
            }

            //if (!string.IsNullOrEmpty(authUserName) && !string.IsNullOrEmpty(authPword))
            //{
            //    client.Authenticator = new HttpBasicAuthenticator(authUserName, authPword);
            //}

            try
            {
                RestResponse<T> response = await client.ExecuteAsync<T>(restRequest);

                result = JsonConvert.DeserializeObject<T>(response.Content);

                return result;
            }

            catch (Exception)
            {
                return result;
            }
        }

        private string GetXTokenHeader(DateTime utcdate, string clientId, string password)
        {
            var date = utcdate.ToString("yyyy-MM-ddHHmmss");
            var data = date + clientId + password;
            return SHA512(data);
        }

        private string SHA512(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);
                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte
                var hashedInputStringBuilder = new StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("x2"));
                return hashedInputStringBuilder.ToString();
            }
        }
    }
}
