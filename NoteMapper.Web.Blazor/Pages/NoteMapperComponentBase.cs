using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NoteMapper.Core;
using NoteMapper.Data.Core.Users;
using NoteMapper.Services.Users;
using NoteMapper.Web.Blazor.Models;

namespace NoteMapper.Web.Blazor.Pages
{
    public abstract class NoteMapperComponentBase : ComponentBase
    {
        protected FeedbackViewModel? Feedback { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [Inject]
        protected IIdentityService IdentityService { get; set; }

        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected IUserLocator UserLocator { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected Task<User?> GetCurrentUserAsync()
        {
            return UserLocator.GetCurrentUserAsync();
        }

        protected Task<Guid?> GetCurrentUserIdAsync()
        {
            return UserLocator.GetCurrentUserIdAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JsRuntime.InvokeVoidAsync("pageLoad");
            }
            
            await base.OnAfterRenderAsync(firstRender);
        }        

        protected void RemoveFeedback()
        {
            SetData(null);
        }

        protected void SetFeedback(ServiceResult result) 
        {
            SetData(FeedbackViewModel.FromServiceResult(result));
        }

        protected void SetError(string message)
        {
            SetData(new FeedbackViewModel
            {
                Message = message,
                Type = FeedbackType.Danger
            });
        }

        private void SetData(FeedbackViewModel? feedback)
        {
            Feedback = feedback;            
            InvokeAsync(() => StateHasChanged());
        }
    }
}
