using System.ComponentModel.DataAnnotations;

namespace NoteMapper.Services.Web.ViewModels.Contact
{
    public class ContactRequestViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";        

        public bool Enabled { get; set; }

        [Required]
        public string Message { get; set; } = "";
    }
}
