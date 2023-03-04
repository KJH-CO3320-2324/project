using NoteMapper.Core;
using NoteMapper.Data.Core.Errors;

namespace NoteMapper.Services.Logging
{
    public interface IErrorLoggingService
    {
        Task<ServiceResult> DeleteErrorAsync(Guid applicationErrorId);

        Task<IReadOnlyCollection<KeyValuePair<string, string>>> GetErrorPropertiesAsync(Guid applicationErrorId);

        Task<IReadOnlyCollection<ApplicationError>> GetErrorsAsync(DateTime from, DateTime? to);

        Task LogErrorMessageAsync(string message);

        Task LogErrorMessageAsync(string message, IDictionary<string, string> data);

        Task LogExceptionAsync(Exception ex);

        Task LogExceptionAsync(Exception ex, string url);
    }
}
