using System.ComponentModel.DataAnnotations;

namespace NoteMapper.Web.Blazor.Models.Account
{
    public class ForgottenPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
    }
}
