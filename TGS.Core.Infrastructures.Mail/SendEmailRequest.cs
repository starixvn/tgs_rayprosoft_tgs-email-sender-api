namespace TGS.Core.Infrastructures.Mail
{
    public class SendEmailRequest
    {
        public string[] Receivers { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}
