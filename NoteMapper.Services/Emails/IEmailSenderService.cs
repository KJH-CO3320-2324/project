using NoteMapper.Core;

namespace NoteMapper.Services.Emails
{
    public interface IEmailSenderService
    {
        Task<ServiceResult> SendEmailAsync(Email email);
    }
}
