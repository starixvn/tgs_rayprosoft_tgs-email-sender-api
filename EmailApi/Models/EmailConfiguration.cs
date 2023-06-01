namespace TGS.EmailApi.Models
{
    public class EmailConfiguration
    {
        public string this[string key] { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public string FromMail { get; set; }

        public string FromName { get; set; }

        public string SmtpServer { get; set; }

        public int Port { get; set; }

        public string Password { get; set; }
    }
}
