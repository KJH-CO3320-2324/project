using System.Data;
using System.Data.SqlClient;
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
            string sql = $"INSERT INTO {TableName} (UserId, RegistrationCodeId) " +
                         "VALUES (@UserId, @RegistrationCodeId) " +
                         $"SELECT TOP 1 {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         "WHERE UserId = @UserId AND RegistrationCodeId = @RegistrationCodeId";

            return ReadSingleAsync(sql, new[]
            {
                GetParameter("@UserId", userRegistrationCode.UserId, SqlDbType.UniqueIdentifier),
                GetParameter("@RegistrationCodeId", userRegistrationCode.RegistrationCodeId, SqlDbType.UniqueIdentifier)
            });
        }

        protected override UserRegistrationCode Map(SqlDataReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
