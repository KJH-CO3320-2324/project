using NoteMapper.Data.Core.Users;

namespace NoteMapper.Services.Users
{
    public interface IUserLocator
    {
        Task<User?> GetCurrentUserAsync();

        Task<Guid?> GetCurrentUserIdAsync();
    }
}
