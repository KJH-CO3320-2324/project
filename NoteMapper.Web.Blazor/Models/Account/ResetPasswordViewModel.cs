using System.ComponentModel.DataAnnotations;

namespace NoteMapper.Web.Blazor.Models.Account
{
    public class ResetPasswordViewModel
    {
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = "";
    }
}
