using NoteMapper.Core;
using NoteMapper.Services.Web.ViewModels.Contact;

namespace NoteMapper.Services.Web.Contact
{
    public interface IContactService
    {
        Task<ContactRequestViewModel> GetContactRequestViewModelAsync();

        Task<ServiceResult> SendContactRequestAsync(ContactRequestViewModel request);
    }
}
