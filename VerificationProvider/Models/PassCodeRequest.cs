using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace VerificationProvider.Models
{
    public class PassCodeRequest
    {
        [Required]
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [Required]
        [EmailAddress]
        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }
    }
}