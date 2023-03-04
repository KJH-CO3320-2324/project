using NoteMapper.Core;
using NoteMapper.Data.Core.Notifications;

namespace NoteMapper.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserNotificationRepository _userNotificationRepository;

        public NotificationService(INotificationRepository notificationRepository,
            IUserNotificationRepository userNotificationRepository)
        {
            _notificationRepository = notificationRepository;
            _userNotificationRepository = userNotificationRepository;
        }

        public Task<ServiceResult> DeleteNotificationAsync(Guid notificationId)
        {
            return _notificationRepository.DeleteAsync(notificationId);
        }

        public async Task<ServiceResult> DismissNotificationAsync(Guid userId, Notification notification)
        {
            UserNotification? userNotification = await _userNotificationRepository.FindAsync(userId, notification.NotificationId);
            if (userNotification == null)
            {
                userNotification = new UserNotification(userId, notification.NotificationId, null, true);
                return await _userNotificationRepository.CreateAsync(userNotification);
            }

            userNotification.Dismissed = true;

            return await _userNotificationRepository.UpdateAsync(userNotification);
        }

        public Task<IReadOnlyCollection<Notification>> GetAllNotificationsAsync()
        {
            return _notificationRepository.GetAllAsync();
        }

        public async Task<IReadOnlyCollection<Notification>> GetUserNotificationsAsync(Guid userId)
        {
            IReadOnlyCollection<Notification> notifications = await _notificationRepository.GetActiveNotificationsAsync();
            if (notifications.Count == 0)
            {
                return notifications;
            }

            List<Notification> toShow = new();

            IReadOnlyCollection<UserNotification> userNotifications = await _userNotificationRepository.GetAsync(userId);
            foreach (Notification notification in notifications)
            {
                UserNotification? userNotification = userNotifications
                    .FirstOrDefault(x => x.NotificationId == notification.NotificationId);

                if (userNotification == null)
                {
                    userNotification = new UserNotification(userId, notification.NotificationId);
                    await _userNotificationRepository.CreateAsync(userNotification);
                }

                if (userNotification.ShouldShow(notification))
                {
                    toShow.Add(notification);
                }
            }

            return toShow;
        }

        public async Task<ServiceResult> HideNotificationAsync(Guid userId, Notification notification)
        {
            UserNotification? userNotification = await _userNotificationRepository.FindAsync(userId, notification.NotificationId);
            if (userNotification == null)
            {
                userNotification = new UserNotification(userId, notification.NotificationId, null, true);
                return await _userNotificationRepository.CreateAsync(userNotification);
            }

            userNotification.HiddenUtc = DateTime.UtcNow;

            return await _userNotificationRepository.UpdateAsync(userNotification);
        }
    }
}
