using NoteMapper.Data.Core.Users;

namespace NoteMapper.Services.Users
{
    public class UserAdminService : IUserAdminService
    {
        private readonly IUserRepository _userRepository;

        public UserAdminService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IReadOnlyCollection<User>> GetUsersAsync()
        {
            IReadOnlyCollection<User> users = await _userRepository.GetUsersAsync();
            return users
                .OrderBy(x => x.Email)
                .ToArray();
        }
    }
}
