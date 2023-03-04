using NoteMapper.Core;

namespace NoteMapper.Data.Core.Users
{
    public interface IUserPreferenceRepository
    {
        Task<IReadOnlyCollection<UserPreference>> GetAsync(Guid userId);

        Task<ServiceResult> UpdateAsync(Guid userId, IReadOnlyCollection<UserPreference> preference);
    }
}
