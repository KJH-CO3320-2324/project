namespace NoteMapper.Services.Web.Security
{
    public class RecaptchaViewModel
    {
        public RecaptchaViewModel(string siteKey)
        {
            SiteKey = siteKey;
        }

        public string SiteKey { get; }
    }
}
