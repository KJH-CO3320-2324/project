using System.Data;
using System.Data.Common;
using NoteMapper.Core;
using NoteMapper.Data.Core.Errors;
using NoteMapper.Data.Core.Users;

namespace NoteMapper.Data.Sql.Repositories.Users
{
    public class UserLoginTokenSqlRepository : SqlRepositoryBase<UserLoginToken>, IUserLoginTokenRepository
    {
        public UserLoginTokenSqlRepository(SqlRepositorySettings settings,
            IApplicationErrorRepository errorRepository)
            : base(settings, errorRepository)
        {
        }

        protected override IReadOnlyCollection<string> SelectColumns => new[]
        {
            "UserId", "ExpiresUtc", "Token"
        };

        protected override string TableName => "UserLoginTokens";

        public Task<UserLoginToken?> CreateAsync(UserLoginToken token)
        {
            string sql = $"INSERT INTO {TableName} (UserLoginTokenId, CreatedUtc, UserId, ExpiresUtc, Token) " +
                         $"VALUES (@UserLoginTokenId, @CreatedUtc, @UserId, @ExpiresUtc, @Token); " +
                         $"SELECT {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         "WHERE UserLoginTokenId = @UserLoginTokenId; ";

            return ReadSingleAsync(sql, new[]
            {
                GetParameter("@UserLoginTokenId", Guid.NewGuid(), DbType.Guid),
                GetParameter("@CreatedUtc", DateTime.UtcNow, DbType.DateTime),
                GetParameter("@UserId", token.UserId, DbType.Guid),
                GetParameter("@ExpiresUtc", token.ExpiresUtc, DbType.DateTime),
                GetParameter("@Token", token.Token, DbType.String)
            });
        }

        public Task<ServiceResult> DeleteAllAsync(Guid userId)
        {
            string sql = $"DELETE {TableName} " +
                         $"WHERE UserId = @UserId";

            return ExecuteQueryAsync(sql, new[]
            {
                GetParameter("@UserId", userId, DbType.Guid)
            });
        }

        public Task<UserLoginToken?> FindAsync(Guid userId, string token)
        {
            string sql = $"SELECT {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         $"WHERE UserId = @UserId AND Token = @Token";

            return ReadSingleAsync(sql, new[]
            {
                GetParameter("@UserId", userId, DbType.Guid),
                GetParameter("@Token", token, DbType.String)
            });
        }

        protected override UserLoginToken Map(DbDataReader reader)
        {
            return new UserLoginToken(reader.GetGuid(0),
                reader.GetDateTime(1),
                reader.GetString(2));
        }
    }
}
