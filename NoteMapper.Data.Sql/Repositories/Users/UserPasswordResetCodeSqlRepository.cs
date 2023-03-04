using System.Data;
using System.Data.SqlClient;
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
            string sql = $"INSERT INTO {TableName} (UserId, ExpiresUtc, Code) " +
                         "VALUES (@UserId, @ExpiresUtc, @Code) " +
                         $"SELECT TOP 1 {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         $"WHERE UserId = @UserId " +
                         "ORDER BY CreatedUtc DESC ";

            return ReadSingleAsync(sql, new[]
            {
                GetParameter("@UserId", resetCode.UserId, SqlDbType.UniqueIdentifier),
                GetParameter("@ExpiresUtc", resetCode.ExpiresUtc, SqlDbType.DateTime),
                GetParameter("@Code", resetCode.Code, SqlDbType.NVarChar)
            });
        }

        public Task<ServiceResult> DeleteAllAsync(Guid userId)
        {
            string sql = $"DELETE {TableName} " +
                         "WHERE UserId = @UserId";

            return ExecuteQueryAsync(sql, new[]
            {
                GetParameter("@UserId", userId, SqlDbType.UniqueIdentifier)
            });
        }

        public Task<UserPasswordResetCode?> FindAsync(Guid userId, string code)
        {
            string sql = $"SELECT TOP 1 {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         "WHERE UserId = @UserId AND Code = @Code";

            return ReadSingleAsync(sql, new[]
            {
                GetParameter("@UserId", userId, SqlDbType.UniqueIdentifier),
                GetParameter("@Code", code, SqlDbType.NVarChar)
            });
        }

        protected override UserPasswordResetCode Map(SqlDataReader reader)
        {
            return new UserPasswordResetCode(reader.GetGuid(0),
                reader.GetDateTime(1),
                reader.GetString(2));
        }
    }
}
