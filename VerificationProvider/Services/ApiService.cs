using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Newtonsoft.Json;
using VerificationProvider.Models;

namespace VerificationProvider.Services
{
    public class ApiService
    {
        private HttpClient _emailProviderHttpClient;
        private HttpClient _tokenProviderHttpClient;
        private readonly IComponentContext _context;

        public ApiService(IComponentContext context)
        {
            _context = context;
        }

        public async Task<bool> SendPassCodeToEmail(EmailRequest emailRequestBody, string authorizationToken)
        {
            _emailProviderHttpClient = _context.ResolveNamed<HttpClient>("EmailProvider");
            var request = CreateRequest("/confirm", emailRequestBody, authorizationToken);

            try
            {
                var response = await _emailProviderHttpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch
            {
                // ignored
            }

            return false;
        }

        public async Task<TokenProviderResponse> GetAuthorizationToken(string apiKey, string providerName)
        {
            _tokenProviderHttpClient = _context.ResolveNamed<HttpClient>("TokenProvider");
            var request = CreateRequest("tokengenerator/generate-email-token");
            request.Headers.Add("x-api-key", apiKey);
            request.Headers.Add("x-provider-name", providerName);
            try
            {
                var response = await _tokenProviderHttpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return JsonConvert.DeserializeObject<TokenProviderResponse>(await response.Content.ReadAsStringAsync());
            }
            catch
            {
                // ignored
            }

            return null;
        }

        private static HttpRequestMessage CreateRequest(string endpointUrl, object body = null, string authorizationToken = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, endpointUrl);
            if (authorizationToken != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authorizationToken);
            }

            if (body == null) return request;

            var jsonBody = JsonConvert.SerializeObject(body);
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            return request;
        }
    }

    public class TokenProviderResponse
    {
        public string Token { get; set; }
    }
}