namespace TGS.EmailApi.Models
{
    public class EmailKitConfiguration
    {
        public string this[string key] { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public string FromMail { get; set; }

        public string FromName { get; set; }

        public string SmtpServer { get; set; }

        public int SmtpPort { get; set; }

        public string SmtpUsername { get; set; }

        public string SmtpPassword { get; set; }
    }
}
