using System.Data;
using System.Data.Common;
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
                GetParameter("@QuestionnaireId", questionnaireId, DbType.Guid)
            });
        }

        public Task<ServiceResult> UpdateQuestionsAsync(
            IReadOnlyCollection<QuestionnaireQuestion> insertQuestions,
            IReadOnlyCollection<QuestionnaireQuestion> updateQuestions,
            IReadOnlyCollection<QuestionnaireQuestion> deleteQuestions)
        {
            List<DbParameter> parameters = new();

            int questionIndex = 0;

            string sql = "";
            if (deleteQuestions.Count > 0)
            {                
                foreach (QuestionnaireQuestion deleteQuestion in deleteQuestions)
                {
                    string parameterName = $"@QuestionId{questionIndex}";

                    sql += $"DELETE {TableName} WHERE QuestionId = {parameterName}; ";

                    parameters.Add(GetParameter(parameterName, deleteQuestion.QuestionId, DbType.Guid));

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
                        GetParameter(parameterName, updateQuestion.QuestionId, DbType.Guid),
                        GetParameter($"@QuestionText{questionIndex}", updateQuestion.QuestionText, DbType.String),
                        GetParameter($"@QuestionType{questionIndex}", (int)updateQuestion.Type, DbType.Int32),
                        GetParameter($"@Required{questionIndex}", updateQuestion.Required, DbType.Boolean),
                        GetParameter($"@MinValue{questionIndex}", updateQuestion.MinValue, DbType.Int32),
                        GetParameter($"@MaxValue{questionIndex}", updateQuestion.MaxValue, DbType.Int32),
                        GetParameter($"@DisplayOrder{questionIndex}", updateQuestion.DisplayOrder, DbType.Int32)
                    });

                    questionIndex++;
                }
            }

            if (insertQuestions.Count > 0)
            {
                foreach (QuestionnaireQuestion insertQuestion in insertQuestions)
                {
                    sql += $"INSERT INTO {TableName} (QuestionId, QuestionnaireId, QuestionText, QuestionTypeId, Required, " +
                                 $"MinValue, MaxValue, DisplayOrder) " +
                                 $"VALUES(@QuestionId{questionIndex}, @QuestionnaireId{questionIndex}, @QuestionText{questionIndex}, " +
                                 $"@QuestionType{questionIndex}, " +
                                 $"@Required{questionIndex}, " +
                                 $"@MinValue{questionIndex}, " +
                                 $"@MaxValue{questionIndex}, " +
                                 $"@DisplayOrder{questionIndex}); ";

                    parameters.AddRange(new[]
                    {
                        GetParameter($"@QuestionId{questionIndex}", Guid.NewGuid(), DbType.Guid),
                        GetParameter($"@QuestionnaireId{questionIndex}", insertQuestion.QuestionnaireId, DbType.Guid),
                        GetParameter($"@QuestionText{questionIndex}", insertQuestion.QuestionText, DbType.String),
                        GetParameter($"@QuestionType{questionIndex}", (int)insertQuestion.Type, DbType.Int32),
                        GetParameter($"@Required{questionIndex}", insertQuestion.Required, DbType.Boolean),
                        GetParameter($"@MinValue{questionIndex}", insertQuestion.MinValue, DbType.Int32),
                        GetParameter($"@MaxValue{questionIndex}", insertQuestion.MaxValue, DbType.Int32),
                        GetParameter($"@DisplayOrder{questionIndex}", insertQuestion.DisplayOrder, DbType.Int32)
                    });

                    questionIndex++;
                }
            }

            return ExecuteQueryAsync(sql, parameters);
        }

        protected override QuestionnaireQuestion Map(DbDataReader reader)
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
