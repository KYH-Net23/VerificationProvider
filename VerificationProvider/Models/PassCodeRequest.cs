using System.ComponentModel.DataAnnotations;

namespace VerificationProvider.Models
{
    public class PassCodeRequest
    {
        [Required]
        public string UserId { get; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; }
    }
}