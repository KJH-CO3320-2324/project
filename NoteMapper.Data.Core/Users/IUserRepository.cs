using NoteMapper.Core;

namespace NoteMapper.Data.Core.Users
{
    public interface IUserRepository
    {
        Task<ServiceResult> ActivateAsync(User user);

        Task<User?> CreateAsync(User user);  
        
        Task<ServiceResult> DeleteAsync(Guid userId);

        Task<User?> FindAsync(Guid userId);

        Task<User?> FindByEmailAsync(string email);

        Task<IReadOnlyCollection<User>> GetUsersAsync();
    }
}