using System.Data;
using System.Data.Common;
using NoteMapper.Core;
using NoteMapper.Data.Core.Errors;
using NoteMapper.Data.Core.Users;

namespace NoteMapper.Data.Sql.Repositories.Users
{
    public class UserPasswordResetCodeSqlRepository : SqlRepositoryBase<UserPasswordResetCode>, IUserPasswordResetCodeRepository
    {
        public UserPasswordResetCodeSqlRepository(SqlRepositorySettings settings,
            IApplicationErrorRepository errorRepository) 
            : base(settings, errorRepository)
        {
        }

        protected override IReadOnlyCollection<string> SelectColumns => new[]
        {
            "UserId", "ExpiresUtc", "Code"
        };

        protected override string TableName => "UserPasswordResetCodes";

        public Task<UserPasswordResetCode?> CreateAsync(UserPasswordResetCode resetCode)
        {
            string sql = $"INSERT INTO {TableName} (UserPasswordResetCodeId, CreatedUtc, UserId, ExpiresUtc, Code) " +
                         "VALUES (@UserPasswordResetCodeId, @CreatedUtc, @UserId, @ExpiresUtc, @Code); " +
                         $"SELECT {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         $"WHERE UserPasswordResetCodeId = @UserPasswordResetCodeId; ";

            return ReadSingleAsync(sql, new[]
            {
                GetParameter("@UserPasswordResetCodeId", Guid.NewGuid(), DbType.Guid),
                GetParameter("@CreatedUtc", DateTime.UtcNow, DbType.DateTime),
                GetParameter("@UserId", resetCode.UserId, DbType.Guid),
                GetParameter("@ExpiresUtc", resetCode.ExpiresUtc, DbType.DateTime),
                GetParameter("@Code", resetCode.Code, DbType.String)
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

        public Task<UserPasswordResetCode?> FindAsync(Guid userId, string code)
        {
            string sql = $"SELECT {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         "WHERE UserId = @UserId AND Code = @Code";

            return ReadSingleAsync(sql, new[]
            {
                GetParameter("@UserId", userId, DbType.Guid),
                GetParameter("@Code", code, DbType.String)
            });
        }

        protected override UserPasswordResetCode Map(DbDataReader reader)
        {
            return new UserPasswordResetCode(reader.GetGuid(0),
                reader.GetDateTime(1),
                reader.GetString(2));
        }
    }
}
