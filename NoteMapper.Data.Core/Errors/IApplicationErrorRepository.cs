using NoteMapper.Core;

namespace NoteMapper.Data.Core.Errors
{
    public interface IApplicationErrorRepository
    {
        Task CreateAsync(ApplicationError error);

        Task<ServiceResult> DeleteErrorAsync(Guid applicationErrorId);

        Task<IReadOnlyCollection<KeyValuePair<string, string>>> GetErrorPropertiesAsync(Guid applicationErrorId);

        Task<IReadOnlyCollection<ApplicationError>> GetErrorsAsync(DateTime from, DateTime to);
    }
}
