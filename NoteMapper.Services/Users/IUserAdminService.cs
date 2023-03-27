using NoteMapper.Core;
using NoteMapper.Data.Core.Users;

namespace NoteMapper.Services.Users
{
    public interface IUserAdminService
    {
        Task<ServiceResult> DeleteUserAsync(Guid userId);

        Task<IReadOnlyCollection<User>> GetUsersAsync();
    }
}
