namespace NoteMapper.Data.Core.Questionnaires
{
    public class QuestionnaireQuestion
    {
        public QuestionnaireQuestion(Guid questionId, Guid questionnaireId, string questionText,
            QuestionType type, bool required, int? minValue, int? maxValue, int displayOrder)
        {
            DisplayOrder = displayOrder;
            MaxValue = maxValue;
            MinValue = minValue;
            QuestionId = questionId;
            QuestionnaireId = questionnaireId;
            QuestionText = questionText;            
            Required = required;
            Type = type;
        }

        public int DisplayOrder { get; }

        public int? MaxValue { get; }

        public int? MinValue { get; }

        public Guid QuestionId { get; }

        public Guid QuestionnaireId { get; }

        public string QuestionText { get; }        

        public bool Required { get; }

        public QuestionType Type { get; }
    }
}
