namespace TGS.EmailApi.Models
{
    public class SendEmailRequest
    {
        public SendEmailRequest(string[] receivers, string subject, string content)
        {
            Receivers = receivers;
            Subject = subject;
            Content = content;
        }
        public string[] Receivers { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}
