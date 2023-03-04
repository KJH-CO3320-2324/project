using NoteMapper.Core;
using NoteMapper.Core.Guitars;
using NoteMapper.Data.Core.Instruments;
using NoteMapper.Data.Core.Users;

namespace NoteMapper.Services.Instruments
{
    public class UserInstrumentService : IUserInstrumentService
    {
        private readonly IInstrumentFactory _instrumentFactory;
        private readonly IUserInstrumentRepository _userInstrumentRepository;

        public UserInstrumentService(IUserInstrumentRepository userInstrumentRepository, 
            IInstrumentFactory instrumentFactory)
        {
            _instrumentFactory = instrumentFactory;
            _userInstrumentRepository = userInstrumentRepository;
        }

        public async Task<ServiceResult> ConvertToDefaultAsync(Guid userId, UserInstrument userInstrument)
        {
            ServiceResult validationResult = await ValidateInstrumentAsync(userId, userInstrument);
            if (!validationResult.Success)
            {
                return validationResult;
            }

            ServiceResult result = await _userInstrumentRepository.CreateDefaultInstrumentAsync(userInstrument);
            return result;
        }

        public async Task<ServiceResult> CreateInstrumentAsync(Guid userId, UserInstrument userInstrument)
        {
            ServiceResult validationResult = await ValidateInstrumentAsync(userId, userInstrument);
            if (!validationResult.Success)
            {
                return validationResult;
            }

            userInstrument.UserInstrumentId = Guid.NewGuid().ToString();

            ServiceResult result = await _userInstrumentRepository.CreateUserInstrumentAsync(userId, userInstrument);
            return result.Success
                ? ServiceResult.Successful($"Instrument '{userInstrument.Name}' created")
                : result;
        }

        public Task<ServiceResult> DeleteDefaultInstrumentAsync(string userInstrumentId)
        {
            return _userInstrumentRepository.DeleteDefaultInstrumentAsync(userInstrumentId);
        }

        public async Task<ServiceResult> DeleteInstrumentAsync(Guid userId, string userInstrumentId)
        {
            UserInstrument? instrument = await _userInstrumentRepository.FindUserInstrumentAsync(userId, userInstrumentId);
            if (instrument == null)
            {
                return ServiceResult.Successful();
            }

            ServiceResult result = await _userInstrumentRepository.DeleteUserInstrumentAsync(userId, userInstrumentId);
            return result.Success
                ? ServiceResult.Successful($"Instrument '{instrument.Name}' deleted")
                : result;
        }

        public async Task<GuitarBase?> FindAsync(Guid? userId, string userInstrumentId)
        {
            UserInstrument? userInstrument = await FindUserInstrumentAsync(userId, userInstrumentId);
            return userInstrument != null
                ? _instrumentFactory.FromUserInstrument(userInstrument)
                : null;
        }

        public Task<UserInstrument?> FindDefaultInstrumentAsync(string userInstrumentId)
        {
            return _userInstrumentRepository.FindDefaultInstrumentAsync(userInstrumentId);            
        }

        public async Task<UserInstrument?> FindUserInstrumentAsync(Guid? userId, string userInstrumentId)
        {            
            UserInstrument? @default = await FindDefaultInstrumentAsync(userInstrumentId);
            if (@default != null)
            {
                return @default;
            }

            if (userId == null)
            {
                return null;
            }

            UserInstrument? userInstrument = await _userInstrumentRepository.FindUserInstrumentAsync(userId.Value, userInstrumentId);
            return userInstrument;
        }

        public async Task<IReadOnlyCollection<GuitarBase>> GetDefaultInstrumentsAsync()
        {
            IReadOnlyCollection<UserInstrument> instruments = await _userInstrumentRepository.GetDefaultInstrumentsAsync();
            return instruments
                .Select(_instrumentFactory.FromUserInstrument)
                .ToArray();
        }

        public UserInstrument GetNewUserInstrument(GuitarType type)
        {
            UserInstrument userInstrument = new()
            {                
                Type = type
            };

            userInstrument.Strings.Add(new UserInstrumentString
            {
                NoteIndex = 0,
                OctaveIndex = 0
            });

            return userInstrument;
        }

        public async Task<IReadOnlyCollection<GuitarBase>> GetUserInstrumentsAsync(Guid userId)
        {
            IReadOnlyCollection<UserInstrument> instruments = await _userInstrumentRepository.GetUserInstrumentsAsync(userId);
            return instruments
                .Select(_instrumentFactory.FromUserInstrument)
                .ToArray();
        }

        public async Task<ServiceResult> UpdateInstrumentAsync(Guid userId, UserInstrument instrument)
        {
            ServiceResult validationResult = await ValidateInstrumentAsync(userId, instrument);
            if (!validationResult.Success)
            {
                return validationResult;
            }

            ServiceResult result = await _userInstrumentRepository.UpdateUserInstrumentAsync(userId, instrument);
            return result.Success
                ? ServiceResult.Successful($"Instrument '{instrument.Name}' updated")
                : result;
        }

        private async Task<ServiceResult> ValidateInstrumentAsync(Guid? userId, UserInstrument instrument)
        {
            if (userId == null)
            {
                return ServiceResult.Failure("Not permitted");
            }

            if (string.IsNullOrEmpty(instrument.Name))
            {
                return ServiceResult.Failure("Instrument name required");
            }

            IReadOnlyCollection<UserInstrument> userInstruments = await _userInstrumentRepository.GetUserInstrumentsAsync(userId.Value);
            if (userInstruments.Any(x => x.UserInstrumentId != instrument.UserInstrumentId && 
                                         string.Equals(x.Name, instrument.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                return ServiceResult.Failure("An instrument with that name already exists");
            }

            return ServiceResult.Successful();
        }
    }
}
