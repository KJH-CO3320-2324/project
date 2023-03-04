namespace NoteMapper.Services.Web.Caching
{
    public interface IClientStorageService
    {
        Task<string?> GetAsync(string key);

        Task SetAsync(string key, string? value);
    }
}
