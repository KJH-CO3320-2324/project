namespace NoteMapper.Data.Core.Users
{
    public interface IRegistrationCodeRepository
    {
        Task<RegistrationCode?> FindAsync(string code);
    }
}
