namespace NoteMapper.Data.Core.Users
{
    public class RegistrationCode
    {
        public RegistrationCode(Guid registrationCodeId, string code, 
            DateTime? expiresUtc) 
        {
            Code = code;
            ExpiresUtc = expiresUtc;
            RegistrationCodeId = registrationCodeId;
        }   

        public string Code { get; }

        public DateTime? ExpiresUtc { get; }

        public Guid RegistrationCodeId { get; }
    }
}
