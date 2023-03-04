using Microsoft.Azure.Cosmos;
using NoteMapper.Core;
using NoteMapper.Data.Core.Errors;
using NoteMapper.Data.Core.Instruments;

namespace NoteMapper.Data.Cosmos.Repositories
{
    public class UserInstrumentAzureCosmosRepository : AzureCosmosRepositoryBase<UserInstruments>, IUserInstrumentRepository
    {
        public UserInstrumentAzureCosmosRepository(AzureCosmosRepositorySettings settings,
            IApplicationErrorRepository applicationErrorRepository)
            : base(settings, applicationErrorRepository)
        {
        }

        protected override string ContainerId => "user-instruments";

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
            using (CosmosClient client = CreateClient())
            {
                Container container = GetContainer(client);

                UserInstruments? entry = await FindAsync(container, DefaultUserId);
                if (entry == null)
                {
                    return Array.Empty<UserInstrument>();
                }

                return entry.Instruments.ToArray();
            }
        }

        public async Task<IReadOnlyCollection<UserInstrument>> GetUserInstrumentsAsync(Guid userId)
        {
            using (CosmosClient client = CreateClient())
            {
                Container container = GetContainer(client);

                UserInstruments? entry = await FindAsync(container, userId.ToString());
                if (entry == null)
                {
                    return Array.Empty<UserInstrument>();
                }

                return entry.Instruments.ToArray();
            }
        }

        public async Task<ServiceResult> UpdateUserInstrumentAsync(Guid userId, UserInstrument userInstrument)
        {
            using (CosmosClient client = CreateClient())
            {
                Container container = GetContainer(client);
                UserInstruments? entry = await FindAsync(container, userId.ToString());
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

                return await UpdateAsync(container, userId.ToString(), entry);
            }
        }

        private async Task<ServiceResult> CreateUserInstrumentAsync(string userId, UserInstrument userInstrument)
        {
            using (CosmosClient client = CreateClient())
            {
                Container container = GetContainer(client);
                UserInstruments? entry = await FindAsync(container, userId);
                if (entry == null)
                {
                    entry = new UserInstruments
                    {
                        Id = userId
                    };

                    ServiceResult createResult = await CreateAsync(container, userId, entry);
                    if (!createResult.Success)
                    {
                        return createResult;
                    }
                }

                entry.Instruments.Add(userInstrument);
                return await UpdateAsync(container, userId, entry);
            }
        }

        private async Task<ServiceResult> DeleteUserInstrumentAsync(string userId, string userInstrumentId)
        {
            using (CosmosClient client = CreateClient())
            {
                Container container = GetContainer(client);
                UserInstruments? entry = await FindAsync(container, userId);
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

                return await UpdateAsync(container, userId, entry);
            }
        }

        private async Task<UserInstrument?> FindUserInstrumentAsync(string userId, string userInstrumentId)
        {
            using (CosmosClient client = CreateClient())
            {
                Container container = GetContainer(client);

                UserInstruments? entry = await FindAsync(container, userId);
                if (entry == null)
                {
                    return null;
                }

                return entry.Instruments
                    .FirstOrDefault(x => string.Equals(x.UserInstrumentId, userInstrumentId, StringComparison.InvariantCultureIgnoreCase));
            }
        }
    }
}
