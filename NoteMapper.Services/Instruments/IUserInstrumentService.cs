using NoteMapper.Core;
using NoteMapper.Core.Guitars;
using NoteMapper.Data.Core.Instruments;

namespace NoteMapper.Services.Instruments
{
    public interface IUserInstrumentService
    {
        Task<ServiceResult> ConvertToDefaultAsync(Guid userId, string userInstrumentId);

        Task<ServiceResult> CreateInstrumentAsync(Guid userId, UserInstrument instrument);

        Task<ServiceResult> DeleteDefaultInstrumentAsync(string userInstrumentId);

        Task<ServiceResult> DeleteInstrumentAsync(Guid userId, string userInstrumentId);

        Task<GuitarBase?> FindAsync(Guid? userId, string userInstrumentId);

        Task<UserInstrument?> FindDefaultInstrumentAsync(string userInstrumentId);

        Task<UserInstrument?> FindUserInstrumentAsync(Guid? userId, string userInstrumentId);

        Task<IReadOnlyCollection<GuitarBase>> GetDefaultInstrumentsAsync();

        UserInstrument GetNewUserInstrument(GuitarType type);

        Task<IReadOnlyCollection<GuitarBase>> GetUserInstrumentsAsync(Guid userId);

        Task<ServiceResult> UpdateDefaultInstrumentAsync(UserInstrument instrument);

        Task<ServiceResult> UpdateInstrumentAsync(Guid userId, UserInstrument instrument);
    }
}
