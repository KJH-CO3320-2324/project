using MongoDB.Driver;
using NoteMapper.Core;
using NoteMapper.Data.Core.Errors;
using NoteMapper.Data.Core.Instruments;
using NoteMapper.Data.Mongo.Models.UserInstruments;

namespace NoteMapper.Data.Mongo.Repositories
{
    public class UserInstrumentMongoRepository : MongoRepositoryBase<UserInstruments, UserInstrumentsDto>, IUserInstrumentRepository
    {
        public UserInstrumentMongoRepository(MongoRepositorySettings settings,
            IApplicationErrorRepository applicationErrorRepository)
            : base(settings, applicationErrorRepository)
        {
        }

        protected override string CollectionId => "user-instruments";

        public Task<ServiceResult> CreateDefaultInstrumentAsync(UserInstrument userInstrument)
        {
            return CreateUserInstrumentAsync(DefaultUserId, userInstrument);
        }

        public Task<ServiceResult> CreateUserInstrumentAsync(Guid userId, UserInstrument userInstrument)
        {
            return CreateUserInstrumentAsync(userId.ToString(), userInstrument);
        }

        public Task<ServiceResult> DeleteDefaultInstrumentAsync(string userInstrumentId)
        {
            return DeleteUserInstrumentAsync(DefaultUserId, userInstrumentId);
        }

        public Task<ServiceResult> DeleteUserInstrumentAsync(Guid userId, string userInstrumentId)
        {
            return DeleteUserInstrumentAsync(userId.ToString(), userInstrumentId);
        }

        public Task<UserInstrument?> FindDefaultInstrumentAsync(string userInstrumentId)
        {
            return FindUserInstrumentAsync(DefaultUserId, userInstrumentId);
        }

        public async Task<UserInstrument?> FindUserInstrumentAsync(Guid userId, string userInstrumentId)
        {
            UserInstrument? userInstrument = await FindUserInstrumentAsync(userId.ToString(), userInstrumentId);
            if (userInstrument != null)
            {
                userInstrument.UserId = userId;
            }

            return userInstrument;
        }

        public async Task<IReadOnlyCollection<UserInstrument>> GetDefaultInstrumentsAsync()
        {
            UserInstruments? entry = await FindAsync(DefaultUserId);
            if (entry == null)
            {
                return Array.Empty<UserInstrument>();
            }

            return entry.Instruments.ToArray();
        }

        public async Task<IReadOnlyCollection<UserInstrument>> GetUserInstrumentsAsync(Guid userId)
        {
            UserInstruments? entry = await FindAsync(userId.ToString());
            if (entry == null)
            {
                return Array.Empty<UserInstrument>();
            }

            return entry.Instruments.ToArray();
        }

        public Task<ServiceResult> UpdateDefaultInstrumentAsync(UserInstrument userInstrument)
        {
            return UpdateUserInstrumentAsync(DefaultUserId, userInstrument);
        }

        public Task<ServiceResult> UpdateUserInstrumentAsync(Guid userId, UserInstrument userInstrument)
        {
            return UpdateUserInstrumentAsync(userId.ToString(), userInstrument);
        }

        protected override UserInstruments MapDto(UserInstrumentsDto dto)
        {
            return new UserInstruments
            {
                Id = dto.Id,
                Instruments = dto.Instruments
            };
        }

        protected override UserInstrumentsDto MapEntity(UserInstruments entity)
        {
            return new UserInstrumentsDto
            {
                Id = entity.Id,
                Instruments = entity.Instruments.ToList()
            };
        }

        private async Task<ServiceResult> CreateUserInstrumentAsync(string userId, UserInstrument userInstrument)
        {
            UserInstruments? entry = await FindAsync(userId);
            if (entry == null)
            {
                entry = new UserInstruments
                {
                    Id = userId
                };

                ServiceResult createResult = await CreateAsync(userId, entry);
                if (!createResult.Success)
                {
                    return createResult;
                }
            }

            entry.Instruments.Add(userInstrument);
            return await UpdateAsync(userId, entry);
        }

        private async Task<ServiceResult> DeleteUserInstrumentAsync(string userId, string userInstrumentId)
        {
            UserInstruments? entry = await FindAsync(userId);
            if (entry == null)
            {
                return ServiceResult.Successful();
            }

            UserInstrument? remove = entry.Instruments
                .FirstOrDefault(x => string.Equals(x.UserInstrumentId, userInstrumentId, StringComparison.InvariantCultureIgnoreCase));
            if (remove == null)
            {
                return ServiceResult.Successful();
            }

            entry.Instruments.Remove(remove);

            return await UpdateAsync(userId, entry);
        }

        private async Task<UserInstrument?> FindUserInstrumentAsync(string userId, string userInstrumentId)
        {
            UserInstruments? entry = await FindAsync(userId);
            if (entry == null)
            {
                return null;
            }

            return entry.Instruments
                .FirstOrDefault(x => string.Equals(x.UserInstrumentId, userInstrumentId, StringComparison.InvariantCultureIgnoreCase));
        }

        private async Task<ServiceResult> UpdateUserInstrumentAsync(string userId, UserInstrument userInstrument)
        {
            UserInstruments? entry = await FindAsync(userId);
            if (entry == null)
            {
                return ServiceResult.Successful();
            }

            for (int i = 0; i < entry.Instruments.Count; i++)
            {
                if (entry.Instruments[i].UserInstrumentId == userInstrument.UserInstrumentId)
                {
                    entry.Instruments[i] = userInstrument;
                }
            }

            return await UpdateAsync(userId, entry);
        }
    }
}
