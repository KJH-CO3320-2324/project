namespace NoteMapper.Emails
{
    public class MailjetEmailSenderServiceSettings
    {
        public string ApiKey { get; set; } = "";

        public string ApiSecret { get; set; } = "";

        public string FromEmail { get; set; } = "";

        public string FromName { get; set; } = "";
    }
}
