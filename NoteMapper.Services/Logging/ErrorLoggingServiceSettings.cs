using NoteMapper.Data.Core.Errors;

namespace NoteMapper.Services.Logging
{
    public class ErrorLoggingServiceSettings
    {
        public ApplicationEnvironment CurrentEnvironment { get; set; }

        public bool Enabled { get; set; }
    }
}
