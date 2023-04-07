using NoteMapper.Data.Core.Errors;

namespace NoteMapper.Data.Json
{
    public class JsonRepositorySettings
    {
        public ApplicationEnvironment CurrentEnvironment { get; set; }

        public string DefaultUserId { get; set; } = "";

        public string FilePath { get; set; } = "";
    }
}
