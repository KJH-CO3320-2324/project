namespace NoteMapper.Data.Core.Errors
{
    public class ApplicationError
    {
        public ApplicationError(ApplicationEnvironment environment, string message)
        {
            CreatedUtc = DateTime.UtcNow;
            Environment = environment;
            Message = message;
            Properties = new Dictionary<string, string>();
        }

        public ApplicationError(ApplicationEnvironment environment, Exception ex)
            : this(environment, ex.Message)
        {
            Type = ex.GetType().Name;

            AddProperty("Exception.StackTrace", ex.StackTrace);
        }

        public ApplicationError(ApplicationEnvironment environment, Exception ex, string url)
            : this(environment, ex)
        {
            AddProperty("Url", url);
        }

        public ApplicationError(Guid applicationErrorId, DateTime createdUtc, ApplicationEnvironment environment, 
            string message, string? type)
        {
            ApplicationErrorId = applicationErrorId;
            CreatedUtc = createdUtc;
            Environment = environment;
            Message = message;
            Type = type;
        }

        public Guid? ApplicationErrorId { get; }        

        public DateTime CreatedUtc { get; }

        public ApplicationEnvironment Environment { get; }

        public string Message { get; }

        public Dictionary<string, string> Properties { get; } = new();

        public string? Type { get; }

        public ApplicationError AddProperty(string key, string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return this;
            }

            Properties.Add(key, value);
            return this;
        }
    }
}
