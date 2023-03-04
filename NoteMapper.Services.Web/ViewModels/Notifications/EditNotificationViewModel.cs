using System.ComponentModel.DataAnnotations;
using NoteMapper.Data.Core.Notifications;

namespace NoteMapper.Services.Web.ViewModels.Notifications
{
    public class EditNotificationViewModel
    {
        public EditNotificationViewModel()
        {
            ContentHtml = "";
        }

        public EditNotificationViewModel(Notification notification)
        {
            Active = notification.Active;
            ContentHtml = notification.ContentHtml;
            Heading = notification.Heading;
            HideForDays = notification.HideForDays;
        }

        public bool Active { get; set; }

        [Required]
        public string ContentHtml { get; set; }

        public string? Heading { get; set; }

        public int HideForDays { get; set; }
    }
}
