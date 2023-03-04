using NoteMapper.Core;
using NoteMapper.Data.Core.Notifications;

namespace NoteMapper.Services.Notifications
{
    public interface INotificationService
    {
        Task<ServiceResult> DeleteNotificationAsync(Guid notificationId);

        Task<ServiceResult> DismissNotificationAsync(Guid userId, Notification notification);

        Task<IReadOnlyCollection<Notification>> GetAllNotificationsAsync();

        Task<IReadOnlyCollection<Notification>> GetUserNotificationsAsync(Guid userId);

        Task<ServiceResult> HideNotificationAsync(Guid userId, Notification notification);
    }
}
