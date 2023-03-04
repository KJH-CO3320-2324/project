using NoteMapper.Core;
using NoteMapper.Core.Users;
using NoteMapper.Data.Core.Users;

namespace NoteMapper.Services.Users
{
    public interface IIdentityService
    {
        Task<ServiceResult> ActivateUserAsync(string email, string code, string password);

        Task<ServiceResult> CanUserSignIn(User? user, string password);

        Task<UserLoginToken?> CreateLoginTokenAsync(Guid userId);

        Task<ServiceResult> DeleteAccountAsync(Guid userId);

        Task<User?> FindUserAsync(Guid userId);

        Task<User?> FindUserAsync(string email);

        RegistrationType GetRegistrationType();

        Task<ServiceResult> RegisterUserAsync(string email, string? code);

        Task<ServiceResult> RequestPasswordResetAsync(string email);

        Task<ServiceResult> ResetPasswordAsync(string email, string code, string newPassword);

        Task<ServiceResult> UpdatePasswordAsync(Guid userId, string oldPassword, string newPassword);

        Task<User?> UseLoginTokenAsync(string email, string token);
    }
}
