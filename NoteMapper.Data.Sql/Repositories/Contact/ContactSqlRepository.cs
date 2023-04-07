using System.Data;
using System.Data.Common;
using NoteMapper.Core;
using NoteMapper.Data.Core.Contact;
using NoteMapper.Data.Core.Errors;

namespace NoteMapper.Data.Sql.Repositories.Contact
{
    public class ContactSqlRepository : SqlRepositoryBase<ContactRequest>, IContactRepository
    {
        public ContactSqlRepository(SqlRepositorySettings settings,
            IApplicationErrorRepository errorRepository)
            : base(settings, errorRepository)
        {
        }

        protected override IReadOnlyCollection<string> SelectColumns => Array.Empty<string>();

        protected override string TableName => "ContactRequests";

        public Task<ServiceResult> CreateAsync(ContactRequest request)
        {
            string sql = $"INSERT INTO {TableName} (ContactRequestId, CreatedUtc, Email, Message) " +
                         "VALUES (@ContactRequestId, @CreatedUtc, @Email, @Message); ";

            return ExecuteQueryAsync(sql, new[]
            {
                GetParameter("@ContactRequestId", Guid.NewGuid(), DbType.Guid),
                GetParameter("@CreatedUtc", DateTime.UtcNow, DbType.DateTime),
                GetParameter("@Email", request.Email, DbType.String),
                GetParameter("@Message", request.Message, DbType.String)
            });
        }

        protected override ContactRequest Map(DbDataReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
