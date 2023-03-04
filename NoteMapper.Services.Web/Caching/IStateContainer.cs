namespace NoteMapper.Services.Web.Caching
{
    public interface IStateContainer
    {
        T? Get<T>(string key);

        void Remove(string key);

        void Set<T>(string key, T value);
    }
}
