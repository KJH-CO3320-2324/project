namespace NoteMapper.Data.Core.Notifications
{
    public class Notification
    {
        public Notification(Guid notificationId, string? heading,
            string contentHtml, bool active, int hideForDays)
        {
            Active = active;
            ContentHtml = contentHtml;
            Heading = heading;
            HideForDays = hideForDays;
            NotificationId = notificationId;
        }

        public bool Active { get; set; }

        public string ContentHtml { get; set; }

        public string? Heading { get; set; }

        public int HideForDays { get; set; }

        public Guid NotificationId { get; }
    }
}
