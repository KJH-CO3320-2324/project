using System.Net;
using NoteMapper.Services.Web;

namespace NoteMapper.Web.Blazor.Services
{
    public class UrlEncoder : IUrlEncoder
    {
        public string UrlEncode(string value)
        {
            return WebUtility.UrlEncode(value) ?? value;
        }
    }
}
