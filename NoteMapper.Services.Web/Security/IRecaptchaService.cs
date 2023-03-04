namespace NoteMapper.Services.Web.Security
{
    public interface IRecaptchaService
    {
        RecaptchaViewModel GetRecaptchaViewModel();

        Task<bool> VerifyAsync(string token);
    }
}
