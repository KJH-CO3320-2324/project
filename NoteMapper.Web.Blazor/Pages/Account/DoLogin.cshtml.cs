using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NoteMapper.Identity.Microsoft;
using NoteMapper.Services.Users;
using User = NoteMapper.Data.Core.Users.User;

namespace NoteMapper.Web.Blazor.Pages.Account
{
    public class DoLoginModel : PageModel
    {
        private readonly IIdentityService _identityService;
        private readonly SignInManager<IdentityUser> _signInManager;

        public DoLoginModel(IIdentityService identityService, SignInManager<IdentityUser> signInManager)
        {
            _identityService = identityService;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnGetAsync(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || 
                string.IsNullOrEmpty(token))
            {
                return Redirect("/");
            }

            User? user = await _identityService.UseLoginTokenAsync(email, token);
            if (user == null)
            {
                return Redirect("/account/login");
            }
            
            IdentityUser identityUser = UserMapper.ToIdentityUser(user);
            await _signInManager.SignInAsync(identityUser, true, "Form");
            return Redirect("/");
        }
    }
}
