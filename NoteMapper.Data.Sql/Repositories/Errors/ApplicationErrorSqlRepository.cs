using System.Data;
using System.Data.SqlClient;
using NoteMapper.Core;
using NoteMapper.Data.Core.Errors;
using NoteMapper.Data.Sql.Extensions;

namespace NoteMapper.Data.Sql.Repositories.Errors
{
    public class ApplicationErrorSqlRepository : IApplicationErrorRepository
    {
        private readonly SqlRepositorySettings _settings;

        public ApplicationErrorSqlRepository(SqlRepositorySettings settings)
        {
            _settings = settings;
        }

        private string TableName => "ApplicationErrors";

        public async Task CreateAsync(ApplicationError error)
        {
            string sql = $"INSERT INTO {TableName} (CreatedUtc, Message, Type, ApplicationEnvironmentId) " +
                         "VALUES (@CreatedUtc, @Message, @Type, @ApplicationEnvironmentId)" +
                         "SELECT TOP 1 ApplicationErrorId " +
                         $"FROM {TableName} " +
                         $"WHERE CreatedUtc = @CreatedUtc AND Message = @Message AND Type = @Type " +
                         $"AND ApplicationEnvironmentId = @ApplicationEnvironmentId ";

            Guid? applicationErrorId = null; 

            using (SqlConnection conn = new(_settings.ConnectionString))
            {
                using (SqlCommand cmd = new(sql, conn))
                {
                    cmd.Parameters.Add(SqlUtils.GetParameter("@ApplicationEnvironmentId", (int)error.Environment, SqlDbType.Int));
                    cmd.Parameters.Add(SqlUtils.GetParameter("@CreatedUtc", error.CreatedUtc, SqlDbType.DateTime));
                    cmd.Parameters.Add(SqlUtils.GetParameter("@Message", error.Message, SqlDbType.NVarChar));
                    cmd.Parameters.Add(SqlUtils.GetParameter("@Type", error.Type, SqlDbType.NVarChar));

                    await conn.OpenAsync();

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        applicationErrorId = reader.GetGuid(0);                                                
                    }                    
                }

                if (applicationErrorId == null) 
                {
                    return;
                }                
            }

            foreach (string key in error.Properties.Keys)
            {
                if (error.Properties[key] == null)
                {
                    continue;
                }

                sql = "INSERT INTO ApplicationErrorProperties (ApplicationErrorId, Name, Value) " +
                      "VALUES (@ApplicationErrorId, @Name, @Value) ";

                using (SqlConnection conn = new(_settings.ConnectionString))
                {
                    using (SqlCommand cmd = new(sql, conn))
                    {
                        await conn.OpenAsync();

                        cmd.Parameters.Add(SqlUtils.GetParameter("@ApplicationErrorId", applicationErrorId, SqlDbType.UniqueIdentifier));
                        cmd.Parameters.Add(SqlUtils.GetParameter("@Name", key, SqlDbType.NVarChar));
                        cmd.Parameters.Add(SqlUtils.GetParameter("@Value", error.Properties[key], SqlDbType.NVarChar));

                        await cmd.ExecuteNonQueryAsync();
                    }
                }                
            }
        }

        public async Task<ServiceResult> DeleteErrorAsync(Guid applicationErrorId)
        {
            string sql = $"DELETE {TableName} WHERE ApplicationErrorId = @ApplicationErrorId";

            using (SqlConnection conn = new SqlConnection(_settings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(SqlUtils.GetParameter("@ApplicationErrorId", applicationErrorId, SqlDbType.UniqueIdentifier));

                    await conn.OpenAsync();

                    await cmd.ExecuteNonQueryAsync();

                    return ServiceResult.Successful();
                }
            }
        }

        public async Task<IReadOnlyCollection<KeyValuePair<string, string>>> GetErrorPropertiesAsync(Guid applicationErrorId)
        {
            string sql = "SELECT Name, Value " +
                         "FROM ApplicationErrorProperties " +
                         "WHERE ApplicationErrorId = @ApplicationErrorId ";

            using (SqlConnection conn = new SqlConnection(_settings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(SqlUtils.GetParameter("@ApplicationErrorId", applicationErrorId, SqlDbType.UniqueIdentifier));

                    await conn.OpenAsync();

                    List<KeyValuePair<string, string>> properties = new();

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        properties.Add(new KeyValuePair<string, string>(
                            reader.GetString(0),
                            reader.GetString(1)));
                    }

                    return properties;
                }
            }
        }

        public async Task<IReadOnlyCollection<ApplicationError>> GetErrorsAsync(DateTime from, DateTime to)
        {
            string sql = "SELECT ApplicationErrorId, CreatedUtc, ApplicationEnvironmentId, Message, Type " +
                         $"FROM {TableName} " +
                         "WHERE CreatedUtc >= @From AND CreatedUtc < @To " +
                         "ORDER BY CreatedUtc DESC ";

            using (SqlConnection conn = new SqlConnection(_settings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddRange(new[]
                    {
                        SqlUtils.GetParameter("@From", from, SqlDbType.DateTime),
                        SqlUtils.GetParameter("@To", to.AddDays(1), SqlDbType.DateTime)
                    });

                    await conn.OpenAsync();

                    List<ApplicationError> errors = new();

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        errors.Add(new ApplicationError(
                            reader.GetGuid(0),
                            reader.GetDateTime(1),
                            (ApplicationEnvironment)reader.GetInt32(2),
                            reader.GetString(3),
                            reader.GetStringOrNull(4)));
                    }

                    return errors;
                }
            }
        }
    }
}
