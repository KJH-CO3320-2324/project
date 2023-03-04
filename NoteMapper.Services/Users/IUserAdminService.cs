using NoteMapper.Data.Core.Users;

namespace NoteMapper.Services.Users
{
    public interface IUserAdminService
    {
        Task<IReadOnlyCollection<User>> GetUsersAsync();
    }
}
