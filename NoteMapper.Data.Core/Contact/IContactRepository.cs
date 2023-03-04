using NoteMapper.Core;

namespace NoteMapper.Data.Core.Contact
{
    public interface IContactRepository
    {
        Task<ServiceResult> CreateAsync(ContactRequest request);
    }
}
