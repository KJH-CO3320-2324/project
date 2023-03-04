namespace NoteMapper.Data.Core.Users
{
    public class UserRegistrationCode
    {
        public UserRegistrationCode(Guid userId, Guid registrationCodeId) 
        { 
            RegistrationCodeId = registrationCodeId;
            UserId = userId;
        }

        public Guid RegistrationCodeId { get; }

        public Guid UserId { get; }
    }
}
