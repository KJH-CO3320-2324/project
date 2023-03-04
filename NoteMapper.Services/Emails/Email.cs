namespace NoteMapper.Services.Emails
{
    public class Email
    {
        public Email(string to, string subject, string bodyHtml, string bodyPlain)
        {
            BodyHtml = bodyHtml;
            BodyText = bodyPlain;
            Subject = subject;
            To = to;
        }

        public string BodyHtml { get; }

        public string BodyText { get; }

        public string Subject { get; }

        public string To { get; }
    }
}
