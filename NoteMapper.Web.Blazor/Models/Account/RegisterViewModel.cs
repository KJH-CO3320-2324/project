using System.ComponentModel.DataAnnotations;
using NoteMapper.Core.Users;

namespace NoteMapper.Web.Blazor.Models.Account
{
    public class RegisterViewModel
    {
        public string? Code { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";

        public RegistrationType RegistrationType { get; set; }

        public bool ShowPassword { get; set; }
    }
}
