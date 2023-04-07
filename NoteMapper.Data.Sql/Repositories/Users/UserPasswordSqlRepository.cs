using System.Data;
using System.Data.Common;
using NoteMapper.Core;
using NoteMapper.Data.Core.Errors;
using NoteMapper.Data.Core.Users;

namespace NoteMapper.Data.Sql.Repositories.Users
{
    public class UserPasswordSqlRepository : SqlRepositoryBase<UserPassword>, IUserPasswordRepository
    {
        public UserPasswordSqlRepository(SqlRepositorySettings settings,
            IApplicationErrorRepository errorRepository)
            : base(settings, errorRepository)
        {
        }

        protected override IReadOnlyCollection<string> SelectColumns => new[]
        {
            "UserId", "Hash", "Salt"
        };

        protected override string TableName => "UserPasswords";

        public Task<UserPassword?> FindAsync(Guid userId)
        {
            string sql = $"SELECT {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         $"WHERE UserId = @UserId ";

            return ReadSingleAsync(sql, new[]
            {
                GetParameter("@UserId", userId, DbType.Guid)
            });
        }

        public Task<ServiceResult> UpdateAsync(UserPassword userPassword)
        {
            string sql = $"UPDATE {TableName} " +
                         "SET Hash = @Hash, Salt = @Salt " +
                         "WHERE UserId = @UserId; " +
                         $"INSERT INTO {TableName} (UserPasswordId, UserId, Hash, Salt) " +
                         "SELECT @UserPasswordId, @UserId, @Hash, @Salt " +
                         $"WHERE NOT EXISTS(SELECT * FROM {TableName} WHERE UserId = @UserId); ";            

            return ExecuteQueryAsync(sql, new[]
            {
                GetParameter("@UserPasswordId", Guid.NewGuid(), DbType.Guid),
                GetParameter("@UserId", userPassword.UserId, DbType.Guid),
                GetParameter("@Hash", userPassword.Hash, DbType.String),
                GetParameter("@Salt", userPassword.Salt, DbType.String)
            });
        }

        protected override UserPassword Map(DbDataReader reader)
        {
            return new UserPassword(reader.GetGuid(0),
                reader.GetString(1),
                reader.GetString(2));
        }
    }
}
