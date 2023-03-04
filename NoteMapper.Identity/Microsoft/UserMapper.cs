using Microsoft.AspNetCore.Identity;
using NoteMapper.Data.Core.Users;

namespace NoteMapper.Identity.Microsoft
{
    public static class UserMapper
    {
        public static IdentityUser ToIdentityUser(this User user)
        {
            IdentityUser identityUser = new(user.Email)
            {
                Email = user.Email,
                EmailConfirmed = true,
                Id = user.UserId.ToString(),                
                UserName = user.Email                
            };            
            
            return identityUser;
        }
    }
}
