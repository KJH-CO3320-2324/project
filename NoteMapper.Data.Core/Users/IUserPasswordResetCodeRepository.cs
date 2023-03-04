using NoteMapper.Core;

namespace NoteMapper.Data.Core.Users
{
    public interface IUserPasswordResetCodeRepository
    {
        Task<UserPasswordResetCode?> CreateAsync(UserPasswordResetCode resetCode);

        Task<ServiceResult> DeleteAllAsync(Guid userId);

        Task<UserPasswordResetCode?> FindAsync(Guid userId, string code);
    }
}
