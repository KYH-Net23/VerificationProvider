using System;
using System.Net.Http;
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

        public async Task<bool> SendPassCodeToEmail(EmailRequest emailRequestBody)
        {
            _emailProviderHttpClient = _context.ResolveNamed<HttpClient>("EmailProvider");
            var request = new HttpRequestMessage(HttpMethod.Post, "/confirm");

            var body = JsonConvert.SerializeObject(emailRequestBody);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await _emailProviderHttpClient.SendAsync(request);
            try
            {
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public async Task<string> CallTokenProvider()
        {
            _tokenProviderHttpClient = _context.ResolveNamed<HttpClient>("TokenProvider");
            var request = new HttpRequestMessage(HttpMethod.Get, "/tokengenerator/generate-email-token");
            var response = await _tokenProviderHttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();  //might not work
            var json = JsonConvert.DeserializeObject<object>(data);
            return json as string;
        }
    }
}