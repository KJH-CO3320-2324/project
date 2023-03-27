using NoteMapper.Core;
using NoteMapper.Data.Core.Instruments;
using NoteMapper.Data.Core.Users;

namespace NoteMapper.Services.Users
{
    public class UserAdminService : IUserAdminService
    {
        private readonly IUserInstrumentRepository _userInstrumentRepository;
        private readonly IUserRepository _userRepository;

        public UserAdminService(IUserRepository userRepository, 
            IUserInstrumentRepository userInstrumentRepository)
        {
            _userInstrumentRepository = userInstrumentRepository;
            _userRepository = userRepository;
        }

        public async Task<ServiceResult> DeleteUserAsync(Guid userId)
        {
            ServiceResult deleteResult = await _userRepository.DeleteAsync(userId);
            if (!deleteResult.Success)
            {
                return ServiceResult.Failure("Error deleting user");
            }

            await _userInstrumentRepository.DeleteUserAsync(userId);
            return ServiceResult.Successful("User deleted");
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
