namespace NoteMapper.Core.IO
{
    public interface IFilePathResolver
    {
        string GetLocalFilePath(string relativePath);
    }
}
