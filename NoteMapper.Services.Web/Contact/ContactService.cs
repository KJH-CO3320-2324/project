using NoteMapper.Core;
using NoteMapper.Data.Core.Contact;
using NoteMapper.Data.Core.Users;
using NoteMapper.Services.Emails;
using NoteMapper.Services.Users;
using NoteMapper.Services.Web.ViewModels.Contact;

namespace NoteMapper.Services.Web.Contact
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;
        private readonly IEmailSenderService _emailSenderService;
        private readonly ContactServiceSettings _settings;
        private readonly IUserLocator _userLocator;

        public ContactService(IContactRepository contactRepository,
            IEmailSenderService emailSenderService,
            ContactServiceSettings settings,
            IUserLocator userLocator)
        {
            _contactRepository = contactRepository;
            _emailSenderService = emailSenderService;
            _settings = settings;
            _userLocator = userLocator;
        }

        public async Task<ContactRequestViewModel> GetContactRequestViewModelAsync()
        {
            User? user = await _userLocator.GetCurrentUserAsync();

            return new ContactRequestViewModel
            {
                Email = user?.Email ?? "",
                Enabled = _settings.Enabled,
                Message = ""
            };
        }

        public async Task<ServiceResult> SendContactRequestAsync(ContactRequestViewModel request)
        {
            if (!_settings.Enabled)
            {
                return ServiceResult.Failure("The contact form is currently closed");
            }

            await _contactRepository.CreateAsync(new ContactRequest
            {
                Email = request.Email,
                Message = request.Message
            });

            string subject = $"{_settings.ApplicationName}: New contact request";
            string bodyPlain = $"From: {request.Email}" + Environment.NewLine + request.Message;

            Email email = new(_settings.ContactEmailAddress, subject, "", bodyPlain);
            ServiceResult sendResult = await _emailSenderService.SendEmailAsync(email);

            return sendResult.Success
                ? ServiceResult.Successful("Your message has been sent")
                : ServiceResult.Failure("An error occurred while sending your request");
        }
    }
}
