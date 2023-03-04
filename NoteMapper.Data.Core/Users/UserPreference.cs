namespace NoteMapper.Data.Core.Users
{
    public class UserPreference
    {
        public UserPreference(UserPreferenceType type, string value)
        {
            Type = type;
            Value = value;
        }

        public UserPreferenceType Type { get; }

        public string Value { get; }
    }
}
