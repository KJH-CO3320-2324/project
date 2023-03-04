using Microsoft.JSInterop;

namespace NoteMapper.Services.Web.Caching
{
    public class ClientStorageService : IClientStorageService
    {
        private readonly IJSRuntime _jsRuntime;

        public ClientStorageService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<string?> GetAsync(string key)
        {
            string? value = await _jsRuntime.InvokeAsync<string?>("window.localStorage.getItem", key);
            return value;
        }

        public async Task SetAsync(string key, string? value)
        {
            await _jsRuntime.InvokeVoidAsync("window.localStorage.setItem", key, value);
        }
    }
}
