namespace NoteMapper.Data.Core.Users
{
    public class UserLoginToken
    {
        public UserLoginToken(Guid userId, DateTime expiresUtc, string token)
        {
            ExpiresUtc = expiresUtc;
            Token = token;
            UserId = userId;
        }

        public DateTime ExpiresUtc { get; }

        public string Token { get; }

        public Guid UserId { get; }
    }
}
