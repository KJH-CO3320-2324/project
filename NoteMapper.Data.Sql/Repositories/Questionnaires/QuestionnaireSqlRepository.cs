using System.Data;
using System.Data.Common;
using NoteMapper.Core;
using NoteMapper.Data.Core.Errors;
using NoteMapper.Data.Core.Questionnaires;
using NoteMapper.Data.Sql.Extensions;

namespace NoteMapper.Data.Sql.Repositories.Questionnaires
{
    public class QuestionnaireSqlRepository : SqlRepositoryBase<Questionnaire>, IQuestionnaireRepository
    {
        public QuestionnaireSqlRepository(SqlRepositorySettings settings,
            IApplicationErrorRepository applicationErrorRepository)
            : base(settings, applicationErrorRepository)
        {
        }

        protected override IReadOnlyCollection<string> SelectColumns => new[]
        {
            "QuestionnaireId", "Name", "ExpiresUtc", "Active", "LinkText", "IntroText"
        };

        protected override string TableName => "Questionnaires";        

        public Task<Questionnaire?> CreateAsync(Questionnaire questionnaire)
        {
            string sql = $"INSERT INTO {TableName} (QuestionnaireId, CreatedUtc, Name, ExpiresUtc, Active, LinkText, IntroText) " +
                         $"VALUES (@QuestionnaireId, @CreatedUtc, @Name, @ExpiresUtc, @Active, @LinkText, @IntroText); " +
                         $"SELECT {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         $"WHERE QuestionnaireId = @QuestionnaireId; ";

            return ReadSingleAsync(sql, new[]
            {
                GetParameter("@QuestionnaireId", Guid.NewGuid(), DbType.Guid),
                GetParameter("@CreatedUtc", DateTime.UtcNow, DbType.DateTime),
                GetParameter("@Name", questionnaire.Name, DbType.String),
                GetParameter("@ExpiresUtc", questionnaire.ExpiresUtc, DbType.DateTime),
                GetParameter("@Active", questionnaire.Active, DbType.Boolean),
                GetParameter("@LinkText", questionnaire.LinkText, DbType.String),
                GetParameter("@IntroText", questionnaire.IntroText, DbType.String)
            });
        }

        public Task<ServiceResult> DeleteAsync(Guid questionnaireId)
        {
            string sql = $"DELETE {TableName} WHERE QuestionnaireId = @QuestionnaireId; ";

            return ExecuteQueryAsync(sql, new[]
            {
                GetParameter("@QuestionnaireId", questionnaireId, DbType.Guid)
            });
        }

        public Task<Questionnaire?> FindAsync(Guid questionnaireId)
        {
            string sql = $"SELECT {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         $"WHERE QuestionnaireId = @QuestionnaireId ";

            return ReadSingleAsync(sql, new[]
            {
                GetParameter("@QuestionnaireId", questionnaireId, DbType.Guid)
            });
        }

        public Task<IReadOnlyCollection<Questionnaire>> GetAllAsync()
        {
            string sql = $"SELECT {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         $"ORDER BY CreatedUtc DESC ";

            return ReadManyAsync(sql, Array.Empty<DbParameter>());
        }

        public Task<Questionnaire?> GetCurrentAsync()
        {
            string sql = $"SELECT {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         $"WHERE Active = 1 AND (ExpiresUtc IS NULL OR ExpiresUtc > @Now) " +
                         $"ORDER BY CreatedUtc DESC ";

            return ReadSingleAsync(sql, new[]
            {
                GetParameter("@Now", DateTime.UtcNow, DbType.DateTime)
            });
        }

        public async Task<IDictionary<Guid, int>> GetQuestionnaireRespondentCountsAsync()
        {
            string sql = "SELECT QuestionnaireId, " +
                         $" (" +
                         "  SELECT COUNT(DISTINCT UserId) " +
                         "  FROM UserQuestionResponses " +
                         "  JOIN QuestionnaireQuestions ON UserQuestionResponses.QuestionId = QuestionnaireQuestions.QuestionId " +
                         $" WHERE QuestionnaireQuestions.QuestionnaireId = {TableName}.QuestionnaireId" +
                         $" )" +
                         $"FROM {TableName} ";

            Func<DbDataReader, KeyValuePair<Guid, int>> map = reader =>
            {
                Guid questionnaireId = reader.GetGuid(0);
                int respondents = reader.GetInt32(1);
                return new KeyValuePair<Guid, int>(questionnaireId, respondents);
            };

            IReadOnlyCollection<KeyValuePair<Guid, int>> responseCounts = await ReadManyAsync(sql, Array.Empty<DbParameter>(), map);
            return new Dictionary<Guid, int>(responseCounts);
        }

        public Task<ServiceResult> UpdateAsync(Questionnaire questionnaire)
        {
            string sql = $"UPDATE {TableName} " +
                         "SET Name = @Name, ExpiresUtc = @ExpiresUtc, Active = @Active," +
                         "  LinkText = @LinkText, IntroText = @IntroText " +
                         $"WHERE QuestionnaireId = @QuestionnaireId ";

            return ExecuteQueryAsync(sql, new[]
            {
                GetParameter("@QuestionnaireId", questionnaire.QuestionnaireId, DbType.Guid),
                GetParameter("@Name", questionnaire.Name, DbType.String),
                GetParameter("@ExpiresUtc", questionnaire.ExpiresUtc, DbType.DateTime),
                GetParameter("@Active", questionnaire.Active, DbType.Boolean),
                GetParameter("@LinkText", questionnaire.LinkText, DbType.String),
                GetParameter("@IntroText", questionnaire.IntroText, DbType.String)
            });
        }

        protected override Questionnaire Map(DbDataReader reader)
        {
            return new Questionnaire(
                reader.GetGuid(0),
                reader.GetString(1),
                reader.GetDateTimeOrNull(2),
                reader.GetBoolean(3),
                reader.GetString(4),
                reader.GetString(5));
        }
    }
}
