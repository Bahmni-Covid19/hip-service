namespace In.ProjectEKA.HipService.UserAuth.Model
{
    public class AuthConfirmRequest
    {
        public string transactionId { get; }
        public string authCode { get; }

        public AuthConfirmRequest(string transactionId, string authCode)
        {
            this.transactionId = transactionId;
            this.authCode = authCode;
        }
    }
}