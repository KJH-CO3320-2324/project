namespace NoteMapper.Data.Core.Users
{
    public class UserActivation
    {
        public UserActivation(Guid userId, DateTime expiresUtc, string code) 
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
