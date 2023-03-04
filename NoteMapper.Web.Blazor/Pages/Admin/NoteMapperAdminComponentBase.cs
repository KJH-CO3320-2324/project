using NoteMapper.Data.Core.Users;

namespace NoteMapper.Web.Blazor.Pages.Admin
{
    public class NoteMapperAdminComponentBase : NoteMapperComponentBase
    {
        protected bool Authorized { get; set; }

        protected override async Task OnInitializedAsync()
        {
            User? user = await GetCurrentUserAsync();
            Authorized = user?.IsAdmin == true;
            if (!Authorized)
            {
                NavigationManager.NavigateTo("/");
                return;
            }

            await base.OnInitializedAsync();
        }
    }
}
