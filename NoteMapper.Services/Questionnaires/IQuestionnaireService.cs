using NoteMapper.Core;
using NoteMapper.Data.Core.Questionnaires;

namespace NoteMapper.Services.Questionnaires
{
    public interface IQuestionnaireService
    {
        Task<ServiceResult> DeleteQuestionnaireAsync(Guid questionnaireId);

        Task<IReadOnlyCollection<Questionnaire>> GetQuestionnairesAsync();

        Task<bool> UserHasFinishedCurrentQuestionnaire(Guid userId);
    }
}
