namespace VerificationProvider.Models
{
    public class EmailRequest
    {
        public string Receiver { get; set; } // our receiver
        public string PassCode { get; set; }
        public string UserId { get; set; }
    }
}