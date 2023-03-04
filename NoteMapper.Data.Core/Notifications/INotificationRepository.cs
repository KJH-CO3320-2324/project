using NoteMapper.Core;

namespace NoteMapper.Data.Core.Notifications
{
    public interface INotificationRepository
    {
        Task<ServiceResult> CreateAsync(Notification notification);

        Task<ServiceResult> DeleteAsync(Guid notificationId);

        Task<Notification?> FindAsync(Guid notificationId);

        Task<IReadOnlyCollection<Notification>> GetActiveNotificationsAsync();

        Task<IReadOnlyCollection<Notification>> GetAllAsync();

        Task<ServiceResult> UpdateAsync(Notification notification);
    }
}
