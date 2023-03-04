using NoteMapper.Core;
using NoteMapper.Data.Core.Questionnaires;

namespace NoteMapper.Services.Questionnaires
{
    public class QuestionnaireService : IQuestionnaireService
    {
        private readonly IQuestionnaireRepository _questionnaireRepository;
        private readonly IQuestionnaireQuestionRepository _questionRepository;
        private readonly IUserQuestionResponseRepository _responseRepository;

        public QuestionnaireService(IQuestionnaireRepository questionnaireRepository,
            IQuestionnaireQuestionRepository questionRepository, IUserQuestionResponseRepository responseRepository)
        {
            _questionnaireRepository = questionnaireRepository;
            _questionRepository = questionRepository;
            _responseRepository = responseRepository;
        }

        public async Task<ServiceResult> DeleteQuestionnaireAsync(Guid questionnaireId)
        {
            ServiceResult result = await _questionnaireRepository.DeleteAsync(questionnaireId);
            return result.Success
                ? ServiceResult.Successful("Questionnaire deleted")
                : ServiceResult.Failure("Questionnaire could not be deleted");
        }

        public Task<IReadOnlyCollection<Questionnaire>> GetQuestionnairesAsync()
        {
            return _questionnaireRepository.GetAllAsync();
        }

        public async Task<bool> UserHasFinishedCurrentQuestionnaire(Guid userId)
        {
            Questionnaire? questionnaire = await _questionnaireRepository.GetCurrentAsync();
            if (questionnaire == null)
            {
                return true;
            }

            IReadOnlyCollection<UserQuestionResponse> responses = await _responseRepository.GetAsync(userId, questionnaire.QuestionnaireId);
            if (responses.Count == 0)
            {
                return false;
            }

            IReadOnlyCollection<QuestionnaireQuestion> questions = await _questionRepository.GetQuestionsAsync(questionnaire.QuestionnaireId);
            return responses.Count == questions.Count;
        }
    }
}
