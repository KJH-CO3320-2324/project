using NoteMapper.Data.Core.Users;

namespace NoteMapper.Web.Blazor.Pages.Admin
{
    public class NoteMapperAdminComponentBase : NoteMapperComponentBase
    {
        protected bool Authorized { get; set; }

        protected User? User { get; private set; }

        protected override async Task OnInitializedAsync()
        {
            User = await GetCurrentUserAsync();
            Authorized = User?.IsAdmin == true;
            if (!Authorized)
            {
                NavigationManager.NavigateTo("/");
                return;
            }

            await base.OnInitializedAsync();
        }
    }
}
