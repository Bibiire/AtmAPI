using Application.ConfigSettings;
using Application.InfraInterfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System.Net.Http;
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
        
        private readonly ILogger<HttpConnection> _logger;
        private DevApiSettings DevApiConfig { get; }
        public HttpConnection(ILogger<HttpConnection> logger, IOptions<DevApiSettings> devApiConfig)
        {
            
            _logger = logger;
            DevApiConfig = devApiConfig.Value;
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
