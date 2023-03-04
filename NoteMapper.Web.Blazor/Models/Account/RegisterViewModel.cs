using System.ComponentModel.DataAnnotations;

namespace NoteMapper.Web.Blazor.Models.Account
{
    public class RegisterViewModel
    {
        public string? Code { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
    }
}
