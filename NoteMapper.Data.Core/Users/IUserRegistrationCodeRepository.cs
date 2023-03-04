namespace NoteMapper.Data.Core.Users
{
    public interface IUserRegistrationCodeRepository
    {
        Task<UserRegistrationCode?> CreateAsync(UserRegistrationCode userRegistrationCode);
    }
}
