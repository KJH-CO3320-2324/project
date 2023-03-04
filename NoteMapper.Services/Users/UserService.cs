using NoteMapper.Data.Core.Users;

namespace NoteMapper.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserPreferenceRepository _userPreferenceRepository;

        public UserService(IUserPreferenceRepository userPreferenceRepository)
        {
            _userPreferenceRepository = userPreferenceRepository;
        }

        public async Task<UserPreferences> GetPreferences(Guid? userId)
        {
            IReadOnlyCollection<UserPreference> userPreferences = userId != null 
                ? await _userPreferenceRepository.GetAsync(userId.Value)
                : Array.Empty<UserPreference>();

            return UserPreferences.FromCollection(userPreferences);
        }

        public async Task UpdateUserPreferences(Guid userId, UserPreferences preferences)
        {
            await _userPreferenceRepository.UpdateAsync(userId, preferences.ToCollection());
        }
    }
}
