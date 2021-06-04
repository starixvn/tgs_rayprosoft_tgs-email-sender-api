
namespace TGS.Core.Infrastructures.Mail
{
    public record SendEmailResponse
    {
        public SendEmailResponse(
            bool isSuccessful = true,
            string message = "")
        {
            IsSuccessful = isSuccessful;
            Message = message;
        }

        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
    }
}
