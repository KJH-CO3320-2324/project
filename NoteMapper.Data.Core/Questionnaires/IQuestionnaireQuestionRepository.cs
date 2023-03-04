using NoteMapper.Core;

namespace NoteMapper.Data.Core.Questionnaires
{
    public interface IQuestionnaireQuestionRepository
    {
        public Task<IReadOnlyCollection<QuestionnaireQuestion>> GetQuestionsAsync(Guid questionnaireId);

        public Task<ServiceResult> UpdateQuestionsAsync(
            IReadOnlyCollection<QuestionnaireQuestion> insertQuestions,
            IReadOnlyCollection<QuestionnaireQuestion> updateQuestions, 
            IReadOnlyCollection<QuestionnaireQuestion> deleteQuestions);
    }
}
