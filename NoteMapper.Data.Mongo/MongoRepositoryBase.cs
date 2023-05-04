using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using NoteMapper.Core;
using NoteMapper.Data.Core.Errors;
using NoteMapper.Data.Mongo.Models;

namespace NoteMapper.Data.Mongo
{
    public abstract class MongoRepositoryBase<T, TDto> where TDto : MongoDto
    {
        private readonly IApplicationErrorRepository _applicationErrorRepository;
        private readonly MongoRepositorySettings _settings;

        protected MongoRepositoryBase(MongoRepositorySettings settings,
            IApplicationErrorRepository applicationErrorRepository)
        {
            _applicationErrorRepository = applicationErrorRepository;
            _settings = settings;
            DefaultUserId = settings.DefaultUserId;
        }

        protected abstract string CollectionId { get; }

        protected string DefaultUserId { get; }

        protected async Task<ServiceResult> CreateAsync(string id, T entity)
        {
            try
            {
                BsonDocument doc = Serialize(entity);

                IMongoCollection<BsonDocument> collection = GetCollection();
                await collection.InsertOneAsync(doc);
                return ServiceResult.Successful();
            }
            catch (Exception ex)
            {
                await LogException(ex, id, "Create");
                return ServiceResult.Failure("Error creating entity");
            }
        }        

        protected async Task<T?> FindAsync(string id)
        {
            try
            {
                IMongoCollection<BsonDocument> collection = GetCollection();
                FilterDefinition<BsonDocument> filter = GetIdFilter(id);
                IAsyncCursor<BsonDocument> cursor = await collection.FindAsync(filter);
                BsonDocument? document = cursor.FirstOrDefault();
                return Deserialize(document);
            }
            catch
            {
                return default;
            }
        }

        protected abstract T MapDto(TDto dto);

        protected abstract TDto MapEntity(T entity);

        protected async Task<ServiceResult> DeleteAsync(string id)
        {
            try
            {
                IMongoCollection<BsonDocument> collection = GetCollection();
                FilterDefinition<BsonDocument> filter = GetIdFilter(id);
                DeleteResult result = await collection.DeleteOneAsync(filter);
                return ServiceResult.Successful();
            }
            catch (Exception ex)
            {
                await LogException(ex, id, "Delete");
                return ServiceResult.Failure("Error deleting document");
            }
        }

        protected async Task<ServiceResult> UpdateAsync(string id, T entity)
        {
            try
            {
                BsonDocument doc = Serialize(entity);

                IMongoCollection<BsonDocument> collection = GetCollection();
                FilterDefinition<BsonDocument> filter = GetIdFilter(id);
                ReplaceOneResult result = await collection.ReplaceOneAsync(filter, doc);
                return ServiceResult.Successful();
            }
            catch (Exception ex)
            {
                await LogException(ex, id, "Update");
                return ServiceResult.Failure("Error updating entity");
            }
        }

        private static FilterDefinition<BsonDocument> GetIdFilter(string id)
        {
            return Builders<BsonDocument>.Filter
                .Eq(MongoUtils.IdColumn, id);
        }

        private T? Deserialize(BsonDocument? doc)
        {
            if (doc == null)
            {
                return default;
            }

            string json = doc.ToString();
            
            TDto? dto = JsonConvert.DeserializeObject<TDto>(json);
            if (dto == null)
            {
                return default;
            }

            T entity = MapDto(dto);
            return entity;
        }

        private IMongoCollection<BsonDocument> GetCollection()
        {
            MongoClient client = new(_settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(_settings.DatabaseId);
            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(CollectionId);
            return collection;
        }

        private async Task LogException(Exception ex, string document, string action)
        {
            ApplicationError error = new(_settings.CurrentEnvironment, ex);
            error.AddProperty("Mongo.CollectionId", CollectionId);
            error.AddProperty("Mongo.Document", document);
            error.AddProperty("Mongo.Action", action);
            await _applicationErrorRepository.CreateAsync(error);
        }

        private BsonDocument Serialize(T entity)
        {
            TDto dto = MapEntity(entity);
            string json = JsonConvert.SerializeObject(dto);
            BsonDocument doc = BsonSerializer.Deserialize<BsonDocument>(json);
            return doc;
        }
    }
}
