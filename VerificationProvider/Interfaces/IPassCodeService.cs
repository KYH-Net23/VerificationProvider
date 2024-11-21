namespace VerificationProvider.Interfaces
{
    public interface IPassCodeService
    {
        bool ValidatePassCode(string passCode, string userId);
        string GeneratePasscode(string userId);
        string RetrievePasscode(string key);
    }
}