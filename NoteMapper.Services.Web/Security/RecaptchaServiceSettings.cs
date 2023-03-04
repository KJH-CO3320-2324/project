namespace NoteMapper.Services.Web.Security
{
    public class RecaptchaServiceSettings
    {
        public RecaptchaServiceSettings(string secretKey, string siteKey,
            string verifyUrl)
        {
            SecretKey = secretKey;
            SiteKey = siteKey;
            VerifyUrl = verifyUrl;
        }
        
        public string SecretKey { get; }

        public string SiteKey { get; }

        public string VerifyUrl { get; }
    }
}
