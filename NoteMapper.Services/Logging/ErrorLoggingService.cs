using NoteMapper.Core;
using NoteMapper.Data.Core.Errors;

namespace NoteMapper.Services.Logging
{
    public class ErrorLoggingService : IErrorLoggingService
    {
        private readonly IApplicationErrorRepository _applicationErrorRepository;
        private readonly ErrorLoggingServiceSettings _settings;

        public ErrorLoggingService(IApplicationErrorRepository applicationErrorRepository, 
            ErrorLoggingServiceSettings settings)
        {
            _applicationErrorRepository = applicationErrorRepository;
            _settings = settings;
        }

        public Task<ServiceResult> DeleteErrorAsync(Guid applicationErrorId)
        {
            return _applicationErrorRepository.DeleteErrorAsync(applicationErrorId);
        }

        public Task<IReadOnlyCollection<KeyValuePair<string, string>>> GetErrorPropertiesAsync(Guid applicationErrorId)
        {
            return _applicationErrorRepository.GetErrorPropertiesAsync(applicationErrorId);
        }

        public Task<IReadOnlyCollection<ApplicationError>> GetErrorsAsync(DateTime from, DateTime? to)
        {
            return _applicationErrorRepository.GetErrorsAsync(from, to ?? DateTime.UtcNow);
        }

        public Task LogErrorMessageAsync(string message)
        {
            return LogErrorMessageAsync(message, new Dictionary<string, string>());
        }

        public Task LogErrorMessageAsync(string message, IDictionary<string, string> data)
        {
            ApplicationError error = new(_settings.CurrentEnvironment, message);
            foreach (string key in data.Keys)
            {
                error.AddProperty(key, data[key]);
            }
            return LogExceptionAsync(error);
        }

        public Task LogExceptionAsync(Exception ex)
        {
            ApplicationError error = new(_settings.CurrentEnvironment, ex);
            return LogExceptionAsync(error);
        }

        public Task LogExceptionAsync(Exception ex, string url)
        {
            ApplicationError error = new(_settings.CurrentEnvironment, ex, url);
            return LogExceptionAsync(error);
        }

        private Task LogExceptionAsync(ApplicationError error)
        {
            if (!_settings.Enabled)
            {
                return Task.CompletedTask;
            }

            return _applicationErrorRepository.CreateAsync(error);
        }
    }
}
