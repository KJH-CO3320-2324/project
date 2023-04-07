using System.Data;
using System.Data.Common;
using NoteMapper.Core;
using NoteMapper.Data.Core.Errors;
using NoteMapper.Data.Sql.Extensions;

namespace NoteMapper.Data.Sql.Repositories.Errors
{
    public class ApplicationErrorSqlRepository : IApplicationErrorRepository
    {
        private readonly DbQueryManager _queryManager;

        public ApplicationErrorSqlRepository(SqlRepositorySettings settings)
        {
            _queryManager = new(settings, null);
        }

        private string TableName => "ApplicationErrors";

        public async Task CreateAsync(ApplicationError error)
        {
            string sql = $"INSERT INTO {TableName} (ApplicationErrorId, CreatedUtc, Message, Type, ApplicationEnvironmentId) " +
                         "VALUES (@ApplicationErrorId, @CreatedUtc, @Message, @Type, @ApplicationEnvironmentId); ";

            Guid applicationErrorId = Guid.NewGuid();

            await _queryManager.ExecuteQueryAsync(sql, new[]
            {
                _queryManager.GetParameter("@ApplicationErrorId", applicationErrorId, DbType.Guid),
                _queryManager.GetParameter("@ApplicationEnvironmentId", (int)error.Environment, DbType.Int32),
                _queryManager.GetParameter("@CreatedUtc", error.CreatedUtc, DbType.DateTime),
                _queryManager.GetParameter("@Message", error.Message, DbType.String),
                _queryManager.GetParameter("@Type", error.Type, DbType.String)
            });

            foreach (string key in error.Properties.Keys)
            {
                if (error.Properties[key] == null)
                {
                    continue;
                }

                sql = "INSERT INTO ApplicationErrorProperties (ApplicationErrorId, Name, Value) " +
                      "VALUES (@ApplicationErrorId, @Name, @Value); ";

                await _queryManager.ExecuteQueryAsync(sql, new[]
                {
                    _queryManager.GetParameter("@ApplicationErrorId", applicationErrorId, DbType.Guid),
                    _queryManager.GetParameter("@Name", key, DbType.String),
                    _queryManager.GetParameter("@Value", error.Properties[key], DbType.String)
                });              
            }
        }

        public Task<ServiceResult> DeleteErrorAsync(Guid applicationErrorId)
        {
            string sql = $"DELETE {TableName} WHERE ApplicationErrorId = @ApplicationErrorId";

            return _queryManager.ExecuteQueryAsync(sql, new[]
            {
                _queryManager.GetParameter("@ApplicationErrorId", applicationErrorId, DbType.Guid)
            });
        }

        public Task<IReadOnlyCollection<KeyValuePair<string, string>>> GetErrorPropertiesAsync(Guid applicationErrorId)
        {
            string sql = "SELECT Name, Value " +
                         "FROM ApplicationErrorProperties " +
                         "WHERE ApplicationErrorId = @ApplicationErrorId ";

            Func<DbDataReader, KeyValuePair<string, string>> map = reader => 
                new KeyValuePair<string, string>(
                    reader.GetString(0),
                    reader.GetString(1));

            return _queryManager.ReadManyAsync(sql, new[]
            {
                _queryManager.GetParameter("@ApplicationErrorId", applicationErrorId, DbType.Guid)
            }, map);
        }

        public Task<IReadOnlyCollection<ApplicationError>> GetErrorsAsync(DateTime from, DateTime to)
        {
            string sql = "SELECT ApplicationErrorId, CreatedUtc, ApplicationEnvironmentId, Message, Type " +
                         $"FROM {TableName} " +
                         "WHERE CreatedUtc >= @From AND CreatedUtc < @To " +
                         "ORDER BY CreatedUtc DESC ";

            Func<DbDataReader, ApplicationError> map = reader => new ApplicationError(
                reader.GetGuid(0),
                reader.GetDateTime(1),
                (ApplicationEnvironment)reader.GetInt32(2),
                reader.GetString(3),
                reader.GetStringOrNull(4));

            return _queryManager.ReadManyAsync(sql, new[]
            {
                _queryManager.GetParameter("@From", from, DbType.DateTime),
                _queryManager.GetParameter("@To", to.AddDays(1), DbType.DateTime)
            }, map);
        }
    }
}
