using NoteMapper.Data.Core.Errors;

namespace NoteMapper.Data.Sql.Repositories
{
    public class SqlRepositorySettings
    {
        public string ConnectionString { get; set; } = "";

        public ApplicationEnvironment CurrentEnvironment { get; set; }

        public bool LogErrors { get; set; }
    }
}
