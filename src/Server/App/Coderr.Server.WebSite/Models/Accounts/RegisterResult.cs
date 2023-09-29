namespace Coderr.Server.WebSite.Models.Accounts
{
    public class RegisterResult
    {
        public bool VerificationIsRequested { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}