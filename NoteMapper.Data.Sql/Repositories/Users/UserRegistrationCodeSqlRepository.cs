using System.Data;
using System.Data.Common;
using NoteMapper.Data.Core.Errors;
using NoteMapper.Data.Core.Users;

namespace NoteMapper.Data.Sql.Repositories.Users
{
    public class UserRegistrationCodeSqlRepository : SqlRepositoryBase<UserRegistrationCode>, IUserRegistrationCodeRepository
    {
        public UserRegistrationCodeSqlRepository(SqlRepositorySettings settings,
            IApplicationErrorRepository errorRepository) 
            : base(settings, errorRepository)
        {
        }

        protected override IReadOnlyCollection<string> SelectColumns => new[]
        {
            "UserId", "RegistrationCodeId"
        };

        protected override string TableName => "UserRegistrationCodes";

        public Task<UserRegistrationCode?> CreateAsync(UserRegistrationCode userRegistrationCode)
        {
            string sql = $"INSERT INTO {TableName} (UserRegistrationCodeId, CreatedUtc, UserId, RegistrationCodeId) " +
                         "VALUES (@UserRegistrationCodeId, @CreatedUtc, @UserId, @RegistrationCodeId); " +
                         $"SELECT {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         "WHERE UserRegistrationCodeId = @UserRegistrationCodeId; ";

            return ReadSingleAsync(sql, new[]
            {
                GetParameter("@UserRegistrationCodeId", Guid.NewGuid(), DbType.Guid),
                GetParameter("@CreatedUtc", DateTime.UtcNow, DbType.DateTime),
                GetParameter("@UserId", userRegistrationCode.UserId, DbType.Guid),
                GetParameter("@RegistrationCodeId", userRegistrationCode.RegistrationCodeId, DbType.Guid)
            });
        }

        protected override UserRegistrationCode Map(DbDataReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
