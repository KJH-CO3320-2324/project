using System.Security.Cryptography;

namespace NoteMapper.Identity.Microsoft
{
    public class CustomPasswordHasher : IPasswordHasher
    {
        private readonly CustomPasswordHasherSettings _settings;

        public CustomPasswordHasher(CustomPasswordHasherSettings settings)
        {
            _settings = settings;
        }

        public string GenerateSalt()
        {
            using (RandomNumberGenerator saltGenerator = RandomNumberGenerator.Create())
            {
                byte[] saltBytes = new byte[_settings.SaltByteSize];
                saltGenerator.GetBytes(saltBytes);
                string salt = EncodeBytes(saltBytes);
                return salt;
            }
        }

        public string HashPassword(string plainText, string salt)
        {
            byte[] saltBytes = DecodeBytes(salt);
            return HashPassword(plainText, saltBytes);
        }        

        private byte[] ComputeHash(string plainText, byte[] salt)
        {
            using (DeriveBytes hashGenerator = GetHashGenerator(plainText, salt))
            {
                return hashGenerator.GetBytes(_settings.HashByteSize);
            }
        }

        private byte[] DecodeBytes(string s)
        {
            return Convert.FromBase64String(s);
        }

        private string EncodeBytes(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        private DeriveBytes GetHashGenerator(string plainText, byte[] salt)
        {
            return new Rfc2898DeriveBytes(plainText, salt, _settings.HashIterations, HashAlgorithmName.SHA256);
        }

        private string HashPassword(string plainText, byte[] salt)
        {
            byte[] hashBytes = ComputeHash(plainText, salt);

            string hash = EncodeBytes(hashBytes);
            return hash;
        }
    }
}