using NoteMapper.Core;

namespace NoteMapper.Data.Core.Questionnaires
{
    public interface IUserQuestionResponseRepository
    {
        Task<IReadOnlyCollection<UserQuestionResponse>> GetAsync(Guid userId, 
            Guid questionnaireId);

        Task<ServiceResult> SaveAsync(IReadOnlyCollection<UserQuestionResponse> responses);
    }
}
