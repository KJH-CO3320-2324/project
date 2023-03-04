namespace NoteMapper.Data.Core.Users
{
    public class UserPassword
    {
        public UserPassword(Guid userId, string hash, string salt)
        {
            Hash = hash;
            Salt = salt;
            UserId = userId;
        }

        public string Hash { get; }

        public string Salt { get; }

        public Guid UserId { get; }
    }
}
