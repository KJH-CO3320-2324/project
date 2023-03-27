using NoteMapper.Core;
using NoteMapper.Core.Guitars;
using NoteMapper.Data.Core.Instruments;

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

        public async Task<ServiceResult> ConvertToDefaultAsync(Guid userId, string userInstrumentId)
        {
            UserInstrument? userInstrument = await _userInstrumentRepository.FindUserInstrumentAsync(userId, userInstrumentId);
            if (userInstrument == null)
            {
                return ServiceResult.Failure("Instrument not found");
            }

            IReadOnlyCollection<UserInstrument> existing = await _userInstrumentRepository.GetDefaultInstrumentsAsync();            

            ServiceResult validationResult = ValidateInstrument(existing, userInstrument);
            if (!validationResult.Success)
            {
                return validationResult;
            }
            
            userInstrument.UserInstrumentId = Guid.NewGuid().ToString();

            ServiceResult result = await _userInstrumentRepository.CreateDefaultInstrumentAsync(userInstrument);
            return result;
        }

        public async Task<ServiceResult> CreateInstrumentAsync(Guid userId, UserInstrument userInstrument)
        {
            IReadOnlyCollection<UserInstrument> existing = await _userInstrumentRepository.GetUserInstrumentsAsync(userId);

            ServiceResult validationResult = ValidateInstrument(existing, userInstrument);
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
            UserInstrument? userInstrument = await FindDefaultInstrumentAsync(userInstrumentId);
            if (userInstrument == null)
            {
                userInstrument = await FindUserInstrumentAsync(userId, userInstrumentId);
            }

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
            if (userId == null)
            {
                return null;
            }

            return await _userInstrumentRepository.FindUserInstrumentAsync(userId.Value, userInstrumentId);
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

        public async Task<ServiceResult> UpdateDefaultInstrumentAsync(UserInstrument instrument)
        {
            IReadOnlyCollection<UserInstrument> existing = await _userInstrumentRepository.GetDefaultInstrumentsAsync();            
            if (!existing.Any(x => x.UserInstrumentId == instrument.UserInstrumentId))
            {
                return ServiceResult.Failure("Instrument not found");
            }

            ServiceResult validationResult = ValidateInstrument(existing, instrument);
            if (!validationResult.Success)
            {
                return validationResult;
            }                        

            ServiceResult result = await _userInstrumentRepository.UpdateDefaultInstrumentAsync(instrument);

            return result.Success
                ? ServiceResult.Successful($"Instrument '{instrument.Name}' updated")
                : result;
        }

        public async Task<ServiceResult> UpdateInstrumentAsync(Guid userId, UserInstrument instrument)
        {
            IReadOnlyCollection<UserInstrument> existing = await _userInstrumentRepository.GetUserInstrumentsAsync(userId);
            if (!existing.Any(x => x.UserInstrumentId == instrument.UserInstrumentId))
            {
                return ServiceResult.Failure("Instrument not found");
            }

            ServiceResult validationResult = ValidateInstrument(existing, instrument);
            if (!validationResult.Success)
            {
                return validationResult;
            }
            
            ServiceResult result = await _userInstrumentRepository.UpdateUserInstrumentAsync(userId, instrument);

            return result.Success
                ? ServiceResult.Successful($"Instrument '{instrument.Name}' updated")
                : result;
        }

        private ServiceResult ValidateInstrument(IReadOnlyCollection<UserInstrument> existing, UserInstrument instrument)
        {
            if (string.IsNullOrEmpty(instrument.Name))
            {
                return ServiceResult.Failure("Instrument name required");
            }

            if (existing.Any(x => x.UserInstrumentId != instrument.UserInstrumentId && 
                                         string.Equals(x.Name, instrument.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                return ServiceResult.Failure("An instrument with that name already exists");
            }

            return ServiceResult.Successful();
        }
    }
}
