namespace NoteMapper.Identity.Microsoft
{
    public class CustomPasswordHasherSettings
    {
        public int HashByteSize { get; set; }

        public int HashIterations { get; set; }

        public int SaltByteSize { get; set; }
    }
}
