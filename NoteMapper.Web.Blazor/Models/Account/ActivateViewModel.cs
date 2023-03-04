using System.ComponentModel.DataAnnotations;

namespace NoteMapper.Web.Blazor.Models.Account
{
    public class ActivateViewModel
    {        
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = "";
    }
}
