using System.Data;
using System.Data.SqlClient;
using NoteMapper.Data.Core.Errors;
using NoteMapper.Data.Core.Users;
using NoteMapper.Data.Sql.Extensions;

namespace NoteMapper.Data.Sql.Repositories.Users
{
    public class RegistrationCodeRepository : SqlRepositoryBase<RegistrationCode>, IRegistrationCodeRepository
    {
        public RegistrationCodeRepository(SqlRepositorySettings settings,
            IApplicationErrorRepository errorRepository) 
            : base(settings, errorRepository)
        {
        }

        protected override IReadOnlyCollection<string> SelectColumns => new[]
        {
            "RegistrationCodeId", "ExpiresUtc", "Code"
        };

        protected override string TableName => "RegistrationCodes";

        public Task<RegistrationCode?> FindAsync(string code)
        {
            string sql = $"SELECT TOP 1 {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         $"WHERE Code = @Code ";

            return ReadSingleAsync(sql, new[]
            {
                GetParameter("@Code", code, SqlDbType.NVarChar)
            });
        }

        protected override RegistrationCode Map(SqlDataReader reader)
        {
            return new RegistrationCode(reader.GetGuid(0),
                reader.GetString(2), 
                reader.GetDateTimeOrNull(1));
        }
    }
}
