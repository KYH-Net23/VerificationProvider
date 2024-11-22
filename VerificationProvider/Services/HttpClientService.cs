using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Newtonsoft.Json;
using VerificationProvider.Models;

namespace VerificationProvider.Services
{
    public class HttpClientService
    {
        private HttpClient _emailProviderHttpClient;
        private HttpClient _tokenProviderHttpClient;
        private readonly IComponentContext _context;

        public HttpClientService(IComponentContext context)
        {
            _context = context;
        }

        public async Task<bool> TryToSendPassCodeToEmail(EmailRequest emailRequestBody, string authorizationToken)
        {
            _emailProviderHttpClient = _context.ResolveNamed<HttpClient>("EmailProvider");
            var requestMessage = CreateHttpRequestMessage("/confirm", emailRequestBody, authorizationToken);

            try
            {
                var response = await _emailProviderHttpClient.SendAsync(requestMessage);
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
            var requestMessage = CreateHttpRequestMessage("tokengenerator/generate-email-token");
            requestMessage.Headers.Add("x-api-key", apiKey);
            requestMessage.Headers.Add("x-provider-name", providerName);
            try
            {
                var response = await _tokenProviderHttpClient.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();
                return JsonConvert.DeserializeObject<TokenProviderResponse>(await response.Content.ReadAsStringAsync());
            }
            catch
            {
                // ignored
            }

            return null;
        }

        private static HttpRequestMessage CreateHttpRequestMessage(string endpointUrl, object body = null, string authorizationToken = null)
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
}