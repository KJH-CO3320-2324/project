using NoteMapper.Core;
using NoteMapper.Services.Web.ViewModels.Notifications;

namespace NoteMapper.Services.Web.Notifications
{
    public interface INotificationViewModelService
    {
        Task<ServiceResult> CreateNotificationAsync(EditNotificationViewModel viewModel);

        Task<EditNotificationViewModel?> GetEditNotificationViewModelAsync(Guid notificationId);

        Task<ServiceResult> UpdateNotificationAsync(Guid notificationId, 
            EditNotificationViewModel viewModel);
    }
}
