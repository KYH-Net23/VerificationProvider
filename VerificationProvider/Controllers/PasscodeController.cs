using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using VerificationProvider.Extensions;
using VerificationProvider.Interfaces;
using VerificationProvider.Models;
using VerificationProvider.Services;

namespace VerificationProvider.Controllers
{
    public class PasscodeController : ApiController
    {
        private readonly IPasscodeService _passcodeService;
        private readonly HttpClientService _httpClientService;
        private readonly string _apiKey = "4f1692fdbfe042e690e07ba3c3be9cf2";  // TODO read from config
        private readonly string _providerName = "VerificationProvider-ApiKey"; // TODO read from config

        public PasscodeController(IPasscodeService passcodeService, HttpClientService httpClientService)
        {
            _passcodeService = passcodeService;
            _httpClientService = httpClientService;
        }

        // Only for debugging purposes
        // TODO remove later
        [HttpGet]
        [ActionName("get")]
        public IHttpActionResult GetPasscode(string key)
        {
            var passcode = _passcodeService.RetrievePasscode(key);

            return Ok(new{ passcode });
        }

        [HttpPost]
        public IHttpActionResult ValidatePasscode([FromBody] ValidatePasscodeRequest request)
        {
            if (!_passcodeService.ValidatePasscodeAndUserId(request.Passcode, request.UserId)) return BadRequest();

            _passcodeService.RemovePasscode(request.UserId);
            return Ok(new {Message = $"Passcode verified. Email is now verified for the user {request.UserId}."});
        }

        [HttpPost]
        [ActionName("generate")]
        public async Task<IHttpActionResult> GeneratePasscode([FromBody] PassCodeRequest passcodeRequest)
        {
            if (passcodeRequest == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tokenProviderResponse = await _httpClientService.GetAuthorizationToken(_apiKey, _providerName);

            if (tokenProviderResponse == null)
                return Unauthorized();

            var emailRequest = passcodeRequest.MapToEmailRequest(_passcodeService.GeneratePasscode(passcodeRequest.EmailAddress));

            var result = await _httpClientService.TryToSendPassCodeToEmail(emailRequest, tokenProviderResponse.Token);

            return result ? (IHttpActionResult)Ok() : BadRequest();
        }
    }
}