using NoteMapper.Core;

namespace NoteMapper.Data.Core.Notifications
{
    public interface IUserNotificationRepository
    {
        Task<ServiceResult> CreateAsync(UserNotification userNotification);

        Task<UserNotification?> FindAsync(Guid userId, Guid notificationId);

        Task<IReadOnlyCollection<UserNotification>> GetAsync(Guid userId);

        Task<ServiceResult> UpdateAsync(UserNotification userNotification);
    }
}
