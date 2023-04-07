using System.Data;
using System.Data.Common;
using NoteMapper.Core;
using NoteMapper.Data.Core.Errors;
using NoteMapper.Data.Core.Users;

namespace NoteMapper.Data.Sql.Repositories.Users
{
    public class UserActivationSqlRepository : SqlRepositoryBase<UserActivation>, IUserActivationRepository
    {
        public UserActivationSqlRepository(SqlRepositorySettings settings,
            IApplicationErrorRepository errorRepository)
            : base(settings, errorRepository)
        {
        }

        protected override IReadOnlyCollection<string> SelectColumns => new[]
        {
            "UserId", "ExpiresUtc", "Code"
        };

        protected override string TableName => "UserActivations";

        public Task<UserActivation?> CreateAsync(UserActivation userActivation)
        {
            string sql = $"INSERT INTO {TableName} (UserActivationId, CreatedUtc, UserId, ExpiresUtc, Code) " +
                         "VALUES (@UserActivationId, @CreatedUtc, @UserId, @ExpiresUtc, @Code); " +
                         $"SELECT {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         $"WHERE UserActivationId = @UserActivationId; ";

            return ReadSingleAsync(sql, new[]
            {
                GetParameter("@UserActivationId", Guid.NewGuid(), DbType.Guid),
                GetParameter("@CreatedUtc", DateTime.UtcNow, DbType.DateTime),
                GetParameter("@UserId", userActivation.UserId, DbType.Guid),
                GetParameter("@ExpiresUtc", userActivation.ExpiresUtc, DbType.DateTime),
                GetParameter("@Code", userActivation.Code, DbType.String)
            });
        }

        public Task<ServiceResult> DeleteAllAsync(Guid userId)
        {
            string sql = $"DELETE {TableName} " +
                         "WHERE UserId = @UserId";

            return ExecuteQueryAsync(sql, new[]
            {
                GetParameter("@UserId", userId, DbType.Guid)
            });
        }

        public Task<UserActivation?> FindAsync(Guid userId, string code)
        {
            string sql = $"SELECT {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         $"WHERE UserId = @UserId AND Code = @Code ";
            
            return ReadSingleAsync(sql, new[]
            {
                GetParameter("@UserId", userId, DbType.Guid),
                GetParameter("@Code", code, DbType.String)
            });
        }

        protected override UserActivation Map(DbDataReader reader)
        {
            return new UserActivation(
                reader.GetGuid(0),
                reader.GetDateTime(1),
                reader.GetString(2));
        }
    }
}
