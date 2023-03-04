using NoteMapper.Core;
using NoteMapper.Data.Core.Notifications;
using NoteMapper.Services.Web.ViewModels.Notifications;

namespace NoteMapper.Services.Web.Notifications
{
    public class NotificationViewModelService : INotificationViewModelService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationViewModelService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public Task<ServiceResult> CreateNotificationAsync(EditNotificationViewModel viewModel)
        {
            Notification notification = new Notification(Guid.Empty, viewModel.Heading, viewModel.ContentHtml,
                viewModel.Active, viewModel.HideForDays);
            return _notificationRepository.CreateAsync(notification);
        }

        public async Task<EditNotificationViewModel?> GetEditNotificationViewModelAsync(Guid notificationId)
        {
            Notification? notification = await _notificationRepository.FindAsync(notificationId);
            return notification != null
                ? new EditNotificationViewModel(notification)
                : null;
        }

        public async Task<ServiceResult> UpdateNotificationAsync(Guid notificationId, EditNotificationViewModel viewModel)
        {
            Notification? existing = await _notificationRepository.FindAsync(notificationId);
            if (existing == null)
            {
                return ServiceResult.Failure("Not found");
            }

            existing.Active = viewModel.Active;
            existing.ContentHtml = viewModel.ContentHtml;
            existing.Heading = viewModel.Heading;
            existing.HideForDays = viewModel.HideForDays;

            ServiceResult result = await _notificationRepository.UpdateAsync(existing);
            return result.Success
                ? ServiceResult.Successful("Notification updated")
                : ServiceResult.Failure("Error updating notification");
        }
    }
}
