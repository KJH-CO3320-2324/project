using System.Net.Http.Json;
using NoteMapper.Services.Logging;

namespace NoteMapper.Services.Web.Security
{
    public class RecaptchaService : IRecaptchaService
    {
        private readonly IErrorLoggingService _errorLoggingService;
        private readonly RecaptchaServiceSettings _settings;

        public RecaptchaService(RecaptchaServiceSettings settings, IErrorLoggingService errorLoggingService)
        {
            _errorLoggingService = errorLoggingService;
            _settings = settings;
        }

        public RecaptchaViewModel GetRecaptchaViewModel()
        {
            return new RecaptchaViewModel(_settings.SiteKey);
        }

        public async Task<bool> VerifyAsync(string token)
        {
            // Docs: https://developers.google.com/recaptcha/docs/verify

            using (HttpClient httpClient = new HttpClient())
            {
                HttpContent formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("secret", _settings.SecretKey),
                    new KeyValuePair<string, string>("response", token)
                });

                HttpResponseMessage response = await httpClient.PostAsync(_settings.VerifyUrl, formContent);                

                try
                {
                    RecaptchaVerifyResponse? verifyResponse = await response.Content.ReadFromJsonAsync<RecaptchaVerifyResponse>();                    

                    bool success = verifyResponse?.Success == true;
                    if (success)
                    {
                        return true;
                    }

                    string rawResponse = await response.Content.ReadAsStringAsync();

                    await _errorLoggingService.LogErrorMessageAsync("Recaptcha failed", new Dictionary<string, string>
                    {
                        { "Response", rawResponse }
                    });

                    return false;
                }                
                catch (Exception ex)
                {
                    await _errorLoggingService.LogExceptionAsync(ex);

                    return false;
                }
            }
        }
    }
}
