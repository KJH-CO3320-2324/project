using System.Data;
using System.Data.SqlClient;
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
            string sql = $"SELECT TOP 1 {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         $"WHERE UserId = @UserId ";

            return ReadSingleAsync(sql, new[]
            {
                GetParameter("@UserId", userId, SqlDbType.UniqueIdentifier)
            });
        }

        public Task<ServiceResult> UpdateAsync(UserPassword userPassword)
        {
            string insertSql = $"INSERT INTO {TableName} (UserId, Hash, Salt) " +
                               "VALUES (@UserId, @Hash, @Salt) ";

            string updateSql = $"UPDATE {TableName} " +
                               "SET Hash = @Hash, Salt = @Salt " +
                               "WHERE UserId = @UserId";

            string sql = $"IF NOT EXISTS(SELECT * FROM {TableName} WHERE UserId = @UserId) " +
                         insertSql + 
                         " ELSE " + 
                         updateSql;

            return ExecuteQueryAsync(sql, new[]
            {
                GetParameter("@UserId", userPassword.UserId, SqlDbType.UniqueIdentifier),
                GetParameter("@Hash", userPassword.Hash, SqlDbType.NVarChar),
                GetParameter("@Salt", userPassword.Salt, SqlDbType.NVarChar)
            });
        }

        protected override UserPassword Map(SqlDataReader reader)
        {
            return new UserPassword(reader.GetGuid(0),
                reader.GetString(1),
                reader.GetString(2));
        }
    }
}
