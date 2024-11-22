namespace VerificationProvider.Interfaces
{
    public interface IPasscodeService
    {
        bool ValidatePasscodeAndUserId(string passCode, string userId);
        string GeneratePasscode(string userId);
        string RetrievePasscode(string key);
        void RemovePasscode(string key);
    }
}