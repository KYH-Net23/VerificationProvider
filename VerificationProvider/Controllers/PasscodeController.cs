using System.Threading.Tasks;
using System.Web.Http;
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
        private readonly string _apiKey = "";  // TODO read from config
        private readonly string _providerName = ""; // TODO read from config

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
        [ActionName("validate")]
        public IHttpActionResult ValidatePasscode([FromBody] string passcode, string userId)
        {
            if (!_passcodeService.ValidatePasscodeAndUserId(passcode, userId)) return BadRequest();

            _passcodeService.RemovePasscode(userId);
            return Ok();
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

            var emailRequest = passcodeRequest.MapToEmailRequest(_passcodeService.GeneratePasscode(passcodeRequest.UserId));

            var result = await _httpClientService.TryToSendPassCodeToEmail(emailRequest, tokenProviderResponse.Token);

            return result ? (IHttpActionResult)Ok() : BadRequest();
        }

    }
}