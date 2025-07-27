namespace BaylongoApi.Services.Email.Setting
{
    public class EmailSettings
    {
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string Provider { get; set; } // "Brevo" o "SendGrid"
        public SendGridSettings SendGrid { get; set; }
        public BrevoSettings Brevo { get; set; }
        public GmailSettings Gmail { get; set; }
    }
    public class SendGridSettings
    {
        public string ApiKey { get; set; }
    }

    public class BrevoSettings
    {
        public string ApiKey { get; set; }
    }
    public class GmailSettings
    {
        public string SmtpServer { get; set; }
        public string SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
    }
}
