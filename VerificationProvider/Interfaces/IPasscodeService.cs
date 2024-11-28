namespace VerificationProvider.Interfaces
{
    public interface IPasscodeService
    {
        bool ValidatePasscodeAndUserId(string passCode, string email);
        string GeneratePasscode(string email);
        string RetrievePasscode(string key);
        void RemovePasscode(string key);
    }
}