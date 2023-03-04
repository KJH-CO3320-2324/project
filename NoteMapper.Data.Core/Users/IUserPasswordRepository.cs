using NoteMapper.Core;

namespace NoteMapper.Data.Core.Users
{
    public interface IUserPasswordRepository
    {
        Task<UserPassword?> FindAsync(Guid userId);

        Task<ServiceResult> UpdateAsync(UserPassword userPassword);
    }
}
