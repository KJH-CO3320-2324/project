using System.Data;
using System.Data.SqlClient;
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
            string sql = $"INSERT INTO {TableName} (Name, ExpiresUtc, Active, LinkText, IntroText) " +
                         $"VALUES (@Name, @ExpiresUtc, @Active, @LinkText, @IntroText); " +
                         $"SELECT TOP 1 {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         $"WHERE Name = @Name ";

            return ReadSingleAsync(sql, new[]
            {
                GetParameter("@Name", questionnaire.Name, SqlDbType.NVarChar),
                GetParameter("@ExpiresUtc", questionnaire.ExpiresUtc, SqlDbType.DateTime),
                GetParameter("@Active", questionnaire.Active, SqlDbType.Bit),
                GetParameter("@LinkText", questionnaire.LinkText, SqlDbType.NVarChar),
                GetParameter("@IntroText", questionnaire.IntroText, SqlDbType.NVarChar)
            });
        }

        public Task<ServiceResult> DeleteAsync(Guid questionnaireId)
        {
            string sql = $"DELETE {TableName} WHERE QuestionnaireId = @QuestionnaireId; ";

            return ExecuteQueryAsync(sql, new[]
            {
                GetParameter("@QuestionnaireId", questionnaireId, SqlDbType.UniqueIdentifier)
            });
        }

        public Task<Questionnaire?> FindAsync(Guid questionnaireId)
        {
            string sql = $"SELECT {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         $"WHERE QuestionnaireId = @QuestionnaireId ";

            return ReadSingleAsync(sql, new[]
            {
                GetParameter("@QuestionnaireId", questionnaireId, SqlDbType.UniqueIdentifier)
            });
        }

        public Task<IReadOnlyCollection<Questionnaire>> GetAllAsync()
        {
            string sql = $"SELECT {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         $"ORDER BY CreatedUtc DESC ";

            return ReadManyAsync(sql, Array.Empty<SqlParameter>());
        }

        public Task<Questionnaire?> GetCurrentAsync()
        {
            string sql = $"SELECT TOP 1 {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         $"WHERE Active = 1 AND (ExpiresUtc IS NULL OR ExpiresUtc > GETUTCDATE()) " +
                         $"ORDER BY CreatedUtc DESC ";

            return ReadSingleAsync(sql, Array.Empty<SqlParameter>());
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

            Func<SqlDataReader, KeyValuePair<Guid, int>> map = reader =>
            {
                Guid questionnaireId = reader.GetGuid(0);
                int respondents = reader.GetInt32(1);
                return new KeyValuePair<Guid, int>(questionnaireId, respondents);
            };

            IReadOnlyCollection<KeyValuePair<Guid, int>> responseCounts = await ReadManyAsync(sql, Array.Empty<SqlParameter>(), map);
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
                GetParameter("@QuestionnaireId", questionnaire.QuestionnaireId, SqlDbType.UniqueIdentifier),
                GetParameter("@Name", questionnaire.Name, SqlDbType.NVarChar),
                GetParameter("@ExpiresUtc", questionnaire.ExpiresUtc, SqlDbType.DateTime),
                GetParameter("@Active", questionnaire.Active, SqlDbType.Bit),
                GetParameter("@LinkText", questionnaire.LinkText, SqlDbType.NVarChar),
                GetParameter("@IntroText", questionnaire.IntroText, SqlDbType.NVarChar)
            });
        }

        protected override Questionnaire Map(SqlDataReader reader)
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
