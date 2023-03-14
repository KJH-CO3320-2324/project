using NoteMapper.Data.Core.Errors;

namespace NoteMapper.Data.Mongo
{
    public class MongoRepositorySettings
    {
        public string ApplicationName { get; set; } = "";

        public string ConnectionString { get; set; } = "";

        public ApplicationEnvironment CurrentEnvironment { get; set; }

        public string DatabaseId { get; set; } = "";

        public string DefaultUserId { get; set; } = "";
    }
}
