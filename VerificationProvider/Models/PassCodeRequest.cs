using System.ComponentModel.DataAnnotations;

namespace VerificationProvider.Models
{
    public class PassCodeRequest
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
    }
}