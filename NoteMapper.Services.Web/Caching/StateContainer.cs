namespace NoteMapper.Services.Web.Caching
{
    public class StateContainer : IStateContainer
    {
        private readonly IDictionary<string, object?> State = new Dictionary<string, object?>();

        public T? Get<T>(string key)
        {
            if (!State.ContainsKey(key))
            {
                return default;
            }

            object? value = State[key];
            if (value is T)
            {
                return (T)value;
            }

            return default;
        }

        public void Remove(string key)
        {
            if (State.ContainsKey(key))
            {
                State.Remove(key);
            }
        }

        public void Set<T>(string key, T value)
        {
            State[key] = value;
        }
    }
}
