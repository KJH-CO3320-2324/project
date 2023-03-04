namespace NoteMapper.Identity
{
    public interface IPasswordHasher
    {
        string GenerateSalt();

        string HashPassword(string plainText, string salt);
    }
}
