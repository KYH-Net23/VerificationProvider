using System.Threading.Tasks;
using System.Web.Http;
using VerificationProvider.Extensions;
using VerificationProvider.Interfaces;
using VerificationProvider.Models;
using VerificationProvider.Services;

namespace VerificationProvider.Controllers
{
    public class PassCodeController : ApiController
    {
        private readonly IPassCodeService _passCodeService;
        private readonly HttpClientService _httpClientService;
        private readonly string _apiKey = "";
        private readonly string _providerName = "";

        public PassCodeController(IPassCodeService passCodeService, HttpClientService httpClientService)
        {
            _passCodeService = passCodeService;
            _httpClientService = httpClientService;
        }

        [HttpGet]
        [ActionName("get")]
        public IHttpActionResult GetPasscode(string key)
        {
            var passcode = _passCodeService.RetrievePasscode(key);

            return Ok(new{ passcode });
        }

        [HttpPost]
        [ActionName("validate")]
        public bool ValidatePasscode([FromBody] string passcode, string userId)
        {
            return _passCodeService.ValidatePassCode(passcode, userId);
        }

        [HttpPost]
        [ActionName("generate")]
        public async Task<IHttpActionResult> GeneratePasscode([FromBody] PassCodeRequest passcodeRequest)
        {
            if (passcodeRequest == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorizationInfo = await _httpClientService.GetAuthorizationToken(_apiKey, _providerName);

            if (authorizationInfo == null)
                return Unauthorized();

            var emailRequest = passcodeRequest.MapToEmailRequest(_passCodeService.GeneratePasscode(passcodeRequest.UserId));

            var result = await _httpClientService.TryToSendPassCodeToEmail(emailRequest, authorizationInfo.Token);

            return result ? (IHttpActionResult)Ok() : BadRequest();
        }

    }
}