namespace NoteMapper.Data.Core.Notifications
{
    public class UserNotification
    {
        public UserNotification(Guid userId, Guid notificationId)
        {
            NotificationId = notificationId;
            UserId = userId;
        }

        public UserNotification(Guid userId, Guid notificationId,
            DateTime? hiddenUtc, bool dismissed)
            : this(userId, notificationId)
        {
            Dismissed = dismissed;
            HiddenUtc = hiddenUtc;            
        }

        public bool Dismissed { get; set; }

        public DateTime? HiddenUtc { get; set; }

        public Guid NotificationId { get; }

        public Guid UserId { get; }

        public bool ShouldShow(Notification notification)
        {
            if (!notification.Active)
            {
                return false;
            }

            if (Dismissed)
            {
                return false;
            }

            if (HiddenUtc == null)
            {
                return true;
            }

            if (notification.HideForDays == 0)
            {
                return HiddenUtc == null;
            }

            DateTime showAfterUtc = HiddenUtc.Value.AddDays(notification.HideForDays);
            return showAfterUtc < DateTime.UtcNow;
        }
    }
}
