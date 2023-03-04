using System.Data;
using System.Data.SqlClient;
using NoteMapper.Core;
using NoteMapper.Data.Core.Errors;
using NoteMapper.Data.Core.Questionnaires;
using NoteMapper.Data.Sql.Extensions;

namespace NoteMapper.Data.Sql.Repositories.Questionnaires
{
    public class UserQuestionResponseSqlRepository : SqlRepositoryBase<UserQuestionResponse>,
        IUserQuestionResponseRepository
    {
        public UserQuestionResponseSqlRepository(SqlRepositorySettings settings, IApplicationErrorRepository errorRepository) 
            : base(settings, errorRepository)
        {
        }

        protected override IReadOnlyCollection<string> SelectColumns => new[]
        {
            "ResponseId", "UserId", "QuestionId", "Value"
        };

        protected override string TableName => "UserQuestionResponses";

        public Task<IReadOnlyCollection<UserQuestionResponse>> GetAsync(Guid userId, 
            Guid questionnaireId)
        {
            string sql = $"SELECT {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         $"JOIN QuestionnaireQuestions ON {TableName}.QuestionId = QuestionnaireQuestions.QuestionId " +
                         "WHERE UserId = @UserId AND QuestionnaireId = @QuestionnaireId ";

            return ReadManyAsync(sql, new[]
            {
                GetParameter("@UserId", userId, SqlDbType.UniqueIdentifier),
                GetParameter("@QuestionnaireId", questionnaireId, SqlDbType.UniqueIdentifier)
            });
        }

        public Task<ServiceResult> SaveAsync(IReadOnlyCollection<UserQuestionResponse> responses)
        {
            if (responses.Count == 0)
            {
                return Task.FromResult(ServiceResult.Successful());
            }

            string sql = "";

            List<SqlParameter> parameters = new();

            for (int i = 0; i < responses.Count; i++)
            {
                UserQuestionResponse response = responses.ElementAt(i);

                parameters.AddRange(new[]
                {
                    GetParameter($"@UserId{i}", response.UserId, SqlDbType.UniqueIdentifier),
                    GetParameter($"@QuestionId{i}", response.QuestionId, SqlDbType.UniqueIdentifier),
                    GetParameter($"@Value{i}", response.Value, SqlDbType.NVarChar)
                });                

                if (response.ResponseId == Guid.Empty)
                {
                    sql += $"INSERT INTO {TableName} (UserId, QuestionId, Value) " +
                           $"VALUES (@UserId{i}, @QuestionId{i}, @Value{i}); ";
                }
                else
                {
                    sql += $"UPDATE {TableName} " +
                           $"SET Value = @Value{i} " +
                           $"WHERE UserId = @UserId{i} AND QuestionId = @QuestionId{i}; ";
                }
            }

            return ExecuteQueryAsync(sql, parameters);
        }

        protected override UserQuestionResponse Map(SqlDataReader reader)
        {
            return new UserQuestionResponse(
                reader.GetGuid(0),
                reader.GetGuid(1),
                reader.GetGuid(2),
                reader.GetStringOrNull(3));
        }
    }
}
