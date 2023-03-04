using System.ComponentModel.DataAnnotations;

namespace NoteMapper.Web.Blazor.Models.Account
{
    public class UpdatePasswordViewModel
    {
        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = "";

        [Required]
        [MinLength(6)]
        public string OldPassword { get; set; } = "";
    }
}
