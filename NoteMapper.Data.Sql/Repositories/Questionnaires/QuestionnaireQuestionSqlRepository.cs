using System.Data;
using System.Data.SqlClient;
using NoteMapper.Core;
using NoteMapper.Data.Core.Errors;
using NoteMapper.Data.Core.Questionnaires;
using NoteMapper.Data.Sql.Extensions;

namespace NoteMapper.Data.Sql.Repositories.Questionnaires
{
    public class QuestionnaireQuestionSqlRepository : SqlRepositoryBase<QuestionnaireQuestion>, IQuestionnaireQuestionRepository
    {
        public QuestionnaireQuestionSqlRepository(SqlRepositorySettings settings, 
            IApplicationErrorRepository applicationErrorRepository)
            : base(settings, applicationErrorRepository)
        {
        }

        protected override IReadOnlyCollection<string> SelectColumns => new[]
        {
            "QuestionId", "QuestionnaireId", "QuestionText", "QuestionTypeId", "Required", "MinValue", "MaxValue", "DisplayOrder"
        };

        protected override string TableName => "QuestionnaireQuestions";

        public Task<IReadOnlyCollection<QuestionnaireQuestion>> GetQuestionsAsync(Guid questionnaireId)
        {
            string sql = $"SELECT {SelectColumnSql} " +
                         $"FROM {TableName} " +
                         $"WHERE QuestionnaireId = @QuestionnaireId " +
                         $"ORDER BY DisplayOrder ";

            return ReadManyAsync(sql, new[]
            {
                GetParameter("@QuestionnaireId", questionnaireId, SqlDbType.UniqueIdentifier)
            });
        }

        public Task<ServiceResult> UpdateQuestionsAsync(
            IReadOnlyCollection<QuestionnaireQuestion> insertQuestions,
            IReadOnlyCollection<QuestionnaireQuestion> updateQuestions,
            IReadOnlyCollection<QuestionnaireQuestion> deleteQuestions)
        {
            List<SqlParameter> parameters = new();

            int questionIndex = 0;

            string sql = "";
            if (deleteQuestions.Count > 0)
            {                
                foreach (QuestionnaireQuestion deleteQuestion in deleteQuestions)
                {
                    string parameterName = $"@QuestionId{questionIndex}";

                    sql += $"DELETE {TableName} WHERE QuestionId = {parameterName}; ";

                    parameters.Add(GetParameter(parameterName, deleteQuestion.QuestionId, SqlDbType.UniqueIdentifier));

                    questionIndex++;
                }
            }

            if (updateQuestions.Count > 0)
            {
                foreach (QuestionnaireQuestion updateQuestion in updateQuestions)
                {
                    string parameterName = $"@QuestionId{questionIndex}";

                    sql += $"UPDATE {TableName} " +
                                 $"SET QuestionText = @QuestionText{questionIndex}, " +
                                 $"QuestionTypeId = @QuestionType{questionIndex}, " +
                                 $"Required = @Required{questionIndex}, " +
                                 $"MinValue = @MinValue{questionIndex}, " +
                                 $"MaxValue = @MaxValue{questionIndex}, " +
                                 $"DisplayOrder = @DisplayOrder{questionIndex} " +
                                 $"WHERE QuestionId = {parameterName}; ";

                    parameters.AddRange(new[]
                    {
                        GetParameter(parameterName, updateQuestion.QuestionId, SqlDbType.UniqueIdentifier),
                        GetParameter($"@QuestionText{questionIndex}", updateQuestion.QuestionText, SqlDbType.NVarChar),
                        GetParameter($"@QuestionType{questionIndex}", (int)updateQuestion.Type, SqlDbType.Int),
                        GetParameter($"@Required{questionIndex}", updateQuestion.Required, SqlDbType.Bit),
                        GetParameter($"@MinValue{questionIndex}", updateQuestion.MinValue, SqlDbType.Int),
                        GetParameter($"@MaxValue{questionIndex}", updateQuestion.MaxValue, SqlDbType.Int),
                        GetParameter($"@DisplayOrder{questionIndex}", updateQuestion.DisplayOrder, SqlDbType.Int)
                    });

                    questionIndex++;
                }
            }

            if (insertQuestions.Count > 0)
            {
                foreach (QuestionnaireQuestion insertQuestion in insertQuestions)
                {
                    sql += $"INSERT INTO {TableName} (QuestionnaireId, QuestionText, QuestionTypeId, Required, " +
                                 $"MinValue, MaxValue, DisplayOrder) " +
                                 $"VALUES(@QuestionnaireId{questionIndex}, @QuestionText{questionIndex}, " +
                                 $"@QuestionType{questionIndex}, " +
                                 $"@Required{questionIndex}, " +
                                 $"@MinValue{questionIndex}, " +
                                 $"@MaxValue{questionIndex}, " +
                                 $"@DisplayOrder{questionIndex}); ";

                    parameters.AddRange(new[]
                    {
                        GetParameter($"@QuestionnaireId{questionIndex}", insertQuestion.QuestionnaireId, SqlDbType.UniqueIdentifier),
                        GetParameter($"@QuestionText{questionIndex}", insertQuestion.QuestionText, SqlDbType.NVarChar),
                        GetParameter($"@QuestionType{questionIndex}", (int)insertQuestion.Type, SqlDbType.Int),
                        GetParameter($"@Required{questionIndex}", insertQuestion.Required, SqlDbType.Bit),
                        GetParameter($"@MinValue{questionIndex}", insertQuestion.MinValue, SqlDbType.Int),
                        GetParameter($"@MaxValue{questionIndex}", insertQuestion.MaxValue, SqlDbType.Int),
                        GetParameter($"@DisplayOrder{questionIndex}", insertQuestion.DisplayOrder, SqlDbType.Int)
                    });

                    questionIndex++;
                }
            }

            return ExecuteQueryAsync(sql, parameters);
        }

        protected override QuestionnaireQuestion Map(SqlDataReader reader)
        {
            return new QuestionnaireQuestion(
                reader.GetGuid(0),
                reader.GetGuid(1),
                reader.GetString(2),
                (QuestionType)reader.GetInt32(3),
                reader.GetBoolean(4),
                reader.GetIntOrNull(5),
                reader.GetIntOrNull(6),
                reader.GetInt32(7));
        }
    }
}
