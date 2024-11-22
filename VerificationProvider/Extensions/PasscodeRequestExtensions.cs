using VerificationProvider.Models;

namespace VerificationProvider.Extensions
{
    public static class PasscodeRequestExtensions
    {
        public static EmailRequest MapToEmailRequest(this PassCodeRequest source, string passcode)
        {
            return new EmailRequest
            {
                Receiver = source.EmailAddress,
                PassCode = passcode,
                UserId = source.UserId
            };
        }
    }
}