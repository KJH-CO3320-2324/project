using Microsoft.AspNetCore.Identity;
using NoteMapper.Data.Core.Users;

namespace NoteMapper.Identity.Microsoft
{
    public class CustomUserStore : IUserStore<IdentityUser>
    {
        private readonly IUserRepository _userRepository;

        public CustomUserStore(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();      
        }

        public Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {

        }

        public async Task<IdentityUser?> FindByIdAsync(string userIdString, CancellationToken cancellationToken)
        {
            Guid.TryParse(userIdString, out Guid userId);
            User? user = await _userRepository.FindAsync(userId);
            return user != null
                ? user.ToIdentityUser()
                : null;
        }

        public async Task<IdentityUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            User? user = await _userRepository.FindByEmailAsync(normalizedUserName);
            return user != null
                ? user.ToIdentityUser()
                : null;
        }

        public Task<string?> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string?> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task SetNormalizedUserNameAsync(IdentityUser user, string? normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetUserNameAsync(IdentityUser user, string? userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
