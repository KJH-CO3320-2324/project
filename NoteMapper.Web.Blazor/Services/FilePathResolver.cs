using NoteMapper.Core.IO;

namespace NoteMapper.Web.Blazor.Services
{
    public class FilePathResolver : IFilePathResolver
    {
        private readonly IWebHostEnvironment env;

        public FilePathResolver(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public string GetLocalFilePath(string relativePath)
        {
            return Path.Combine(env.ContentRootPath, relativePath);
        }
    }
}
