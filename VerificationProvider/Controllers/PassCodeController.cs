using System.Threading.Tasks;
using System.Web.Http;
using VerificationProvider.Interfaces;
using VerificationProvider.Models;
using VerificationProvider.Services;

namespace VerificationProvider.Controllers
{
    public class PassCodeController : ApiController
    {
        private readonly IPassCodeService _passCodeService;
        private readonly ApiService _apiService;

        public PassCodeController(IPassCodeService passCodeService, ApiService apiService)
        {
            _passCodeService = passCodeService;
            _apiService = apiService;
        }

        [HttpGet]
        [ActionName("get")]
        public IHttpActionResult GetPasscode(string key)
        {
            var code = _passCodeService.RetrievePasscode(key);

            return Ok(new{ code });
        }

        // This request comes from identity provider
        // Gets a token from the token provider
        // This then sends a request to the email provider
        [HttpPost]
        [ActionName("generate")]
        public async Task<IHttpActionResult> GeneratePasscode([FromBody] PassCodeRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var randomPasscode = _passCodeService.GeneratePasscode(request.UserId);

            // get token for authorization
            // var token = ....

            var emailRequest = new EmailRequest
            {
                Receiver = request.EmailAddress,
                PassCode = randomPasscode,
                UserId = request.UserId
            };

            var result = await _apiService.SendPassCodeToEmail(emailRequest);

            if (result)
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}