using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.JSInterop;

namespace NoteMapper.Web.Blazor.Extensions
{
    public static class NavigationManagerExtensions
    {
        public static async Task SetQueryStringValuesAsync(this NavigationManager navigationManager, IDictionary<string, string?> values,
            IJSRuntime jsRuntime)
        {
            Uri uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);

            IDictionary<string, StringValues> queryString = QueryHelpers.ParseQuery(uri.Query);
            foreach (string key in values.Keys)
            {
                queryString[key] = WebUtility.UrlEncode(values[key]);
            }

            string path = uri.GetLeftPart(UriPartial.Path);            
            string updatedUrl = QueryHelpers.AddQueryString(path, queryString);

            try
            {
                // NavigationManager.NavigateTo triggers a scroll to top of page
                // Temporarily disable the next scroll in javascript
                await jsRuntime.InvokeVoidAsync("disableNextScroll");
            }            
            catch
            {

            }

            navigationManager.NavigateTo(updatedUrl);
        }

        public static bool TryGetQueryStringValue(this NavigationManager navigationManager, string key, out string value)
        {            
            Uri uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);

            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue(key, out StringValues queryValue))
            {
                value = queryValue.ToString();
                return true;
            }

            value = "";
            return false;
        }
    }
}