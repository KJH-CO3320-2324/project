using NoteMapper.Core;
using NoteMapper.Services.Web.ViewModels.Questionnaires;

namespace NoteMapper.Services.Web.Questionnaires
{
    public interface IQuestionnaireViewModelService
    {
        Task<ServiceResult> CreateQuestionnaireAsync(EditQuestionnaireViewModel viewModel);

        Task<EditQuestionnaireViewModel?> GetEditQuestionnaireViewModel(Guid questionnaireId);

        Task<QuestionnaireResponsesViewModel?> GetLatestQuestionnaireResponsesAsync(
            Guid userId);

        Task<IReadOnlyCollection<ListQuestionnaireViewModel>> GetQuestionnairesAsync();

        Task<ServiceResult> SaveResponsesAsync(Guid userId, 
            QuestionnaireResponsesViewModel responses);

        Task<ServiceResult> UpdateQuestionnaireAsync(Guid questionnaireId, 
            EditQuestionnaireViewModel viewModel);
    }
}
