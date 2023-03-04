namespace NoteMapper.Data.Core.Users
{
    public class UserPasswordResetCode
    {
        public UserPasswordResetCode(Guid userId, DateTime expiresUtc, string code)
        {
            Code = code;
            ExpiresUtc = expiresUtc;
            UserId = userId;
        }

        public string Code { get; }

        public DateTime ExpiresUtc { get; }

        public Guid UserId { get; }
    }
}
