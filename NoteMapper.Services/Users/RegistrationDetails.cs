using NoteMapper.Core.Users;

namespace NoteMapper.Services.Users
{
    public class RegistrationDetails
    {
        public RegistrationDetails(RegistrationType registrationType, bool showPassword)
        {
            RegistrationType = registrationType;
            ShowPassword = showPassword;
        }

        public RegistrationType RegistrationType { get; }

        public bool ShowPassword { get; }
    }
}
