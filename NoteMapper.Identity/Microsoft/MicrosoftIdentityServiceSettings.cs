using NoteMapper.Core.Users;

namespace NoteMapper.Identity.Microsoft
{
    public class MicrosoftIdentityServiceSettings
    {
        public int ActivationCodeExpiresAfterMinutes { get; set; }

        public string ActivationUrl { get; set; } = "";

        public string ApplicationName { get; set; } = "";

        public int LoginTokenExpiresAfterSeconds { get; set; }

        public int PasswordResetCodeExpiresAfterHours { get; set; }

        public string PasswordResetUrl { get; set; } = "";

        public RegistrationType RegistrationType { get; set; }
    }
}
