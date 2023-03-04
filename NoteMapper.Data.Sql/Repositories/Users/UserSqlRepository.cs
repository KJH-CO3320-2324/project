using System.Data;
using System.Data.SqlClient;
using NoteMapper.Core;
using NoteMapper.Data.Core.Errors;
using NoteMapper.Data.Core.Users;
using NoteMapper.Data.Sql.Extensions;

namespace NoteMapper.Data.Sql.Repositories.Users
{
    public class UserSqlRepository : SqlRepositoryBase<User>, IUserRepository
    {
        public UserSqlRepository(SqlRepositorySettings settings,
            IApplicationErrorRepository errorRepository)
            : base(settings, errorRepository)
        {
        }

        protected override IReadOnlyCollection<string> SelectColumns => new[]
        {
            "UserId", "CreatedUtc", "Email", "ActivatedUtc", "PreventEmails", "IsAdmin"
        };

        protected override string TableName => "Users";

        public Task<ServiceResult> ActivateAsync(User user)
        {
            string sql = $"UPDATE {TableName} " +
                         "SET ActivatedUtc = @ActivatedUtc " +
                         "WHERE UserId = @UserId ";

            return ExecuteQueryAsync(sql, new[]
            {
                GetParameter("@ActivatedUtc", user.ActivatedUtc, SqlDbType.DateTime),
                GetParameter("@UserId", user.UserId, SqlDbType.UniqueIdentifier)
            });
        }

        public Task<User?> CreateAsync(User user)
        {
            string sql = $"INSERT INTO {TableName} (CreatedUtc, Email) " +
                         "VALUES (@CreatedUtc, @Email)" +
                         $"SELECT TOP 1 {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         $"WHERE Email = @Email";

            return ReadSingleAsync(sql, new[]
            {
                GetParameter("@CreatedUtc", user.CreatedUtc, SqlDbType.DateTime),
                GetParameter("@Email", user.Email, SqlDbType.NVarChar)
            });
        }

        public Task<ServiceResult> DeleteAsync(Guid userId)
        {
            string sql = $"DELETE {TableName} " +
                         "WHERE UserId = @UserId";

            return ExecuteQueryAsync(sql, new[]
            {
                GetParameter("@UserId", userId, SqlDbType.UniqueIdentifier)
            });
        }

        public Task<User?> FindAsync(Guid userId)
        {
            string sql = $"SELECT TOP 1 {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         $"WHERE UserId = @UserId";

            return ReadSingleAsync(sql, new[]
            {
                GetParameter("@UserId", userId, SqlDbType.UniqueIdentifier)
            });
        }

        public Task<User?> FindByEmailAsync(string email)
        {
            string sql = $"SELECT TOP 1 {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         $"WHERE Email = @Email";

            return ReadSingleAsync(sql, new[]
            {
                GetParameter("@Email", email, SqlDbType.NVarChar)
            });
        }

        public Task<IReadOnlyCollection<User>> GetUsersAsync()
        {
            string sql = $"SELECT {SelectColumnSql} " +
                         $"FROM {TableName} ";

            return ReadManyAsync(sql, Array.Empty<SqlParameter>());
        }

        protected override User Map(SqlDataReader reader)
        {
            return new User(reader.GetGuid(0),
                reader.GetDateTime(1),
                reader.GetString(2),
                reader.GetDateTimeOrNull(3),
                reader.GetBoolean(4),
                reader.GetBoolean(5));
        }
    }
}
