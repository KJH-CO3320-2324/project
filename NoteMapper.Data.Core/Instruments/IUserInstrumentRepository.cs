using NoteMapper.Core;

namespace NoteMapper.Data.Core.Instruments
{
    public interface IUserInstrumentRepository
    {
        Task<ServiceResult> CreateDefaultInstrumentAsync(UserInstrument userInstrument);

        Task<ServiceResult> CreateUserInstrumentAsync(Guid userId, UserInstrument userInstrument);

        Task<ServiceResult> DeleteDefaultInstrumentAsync(string userInstrumentId);

        Task<ServiceResult> DeleteUserAsync(Guid userId);

        Task<ServiceResult> DeleteUserInstrumentAsync(Guid userId, string userInstrumentId);

        Task<UserInstrument?> FindDefaultInstrumentAsync(string userInstrumentId);

        Task<UserInstrument?> FindUserInstrumentAsync(Guid userId, string userInstrumentId);

        Task<IReadOnlyCollection<UserInstrument>> GetDefaultInstrumentsAsync();

        Task<IReadOnlyCollection<UserInstrument>> GetUserInstrumentsAsync(Guid userId);

        Task<ServiceResult> UpdateDefaultInstrumentAsync(UserInstrument userInstrument);

        Task<ServiceResult> UpdateUserInstrumentAsync(Guid userId, UserInstrument userInstrument);
    }
}
