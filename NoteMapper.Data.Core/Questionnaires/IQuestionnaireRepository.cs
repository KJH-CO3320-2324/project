using NoteMapper.Core;

namespace NoteMapper.Data.Core.Questionnaires
{
    public interface IQuestionnaireRepository
    {
        Task<Questionnaire?> CreateAsync(Questionnaire questionnaire);

        Task<ServiceResult> DeleteAsync(Guid questionnaireId);

        Task<Questionnaire?> FindAsync(Guid questionnaireId);

        Task<IReadOnlyCollection<Questionnaire>> GetAllAsync();

        Task<Questionnaire?> GetCurrentAsync();

        Task<IDictionary<Guid, int>> GetQuestionnaireRespondentCountsAsync();

        Task<ServiceResult> UpdateAsync(Questionnaire questionnaire);
    }
}
