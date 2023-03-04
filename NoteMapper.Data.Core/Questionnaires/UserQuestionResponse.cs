namespace NoteMapper.Data.Core.Questionnaires
{
    public class UserQuestionResponse
    {
        public UserQuestionResponse(Guid responseId, Guid userId, Guid questionId, string? value)
        {
            QuestionId = questionId;
            ResponseId = responseId;
            UserId = userId;
            Value = value;
        }

        public Guid QuestionId { get; }

        public Guid ResponseId { get; }

        public Guid UserId { get; }

        public string? Value { get; set; }
    }
}
