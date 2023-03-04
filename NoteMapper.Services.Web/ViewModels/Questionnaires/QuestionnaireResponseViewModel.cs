using NoteMapper.Data.Core.Questionnaires;
using NoteMapper.Services.Web.ViewModels.DataAnnotations;

namespace NoteMapper.Services.Web.ViewModels.Questionnaires
{
    public class QuestionnaireResponseViewModel
    {
        public QuestionnaireResponseViewModel(QuestionnaireQuestion question, 
            UserQuestionResponse? response)
        {
            MaxValue = question.MaxValue;
            MinValue = question.MinValue;
            QuestionId = question.QuestionId;
            QuestionText = question.QuestionText;
            QuestionType = question.Type;            
            Required = question.Required;
            Value = response?.Value ?? "";            
        }

        public int? MaxValue { get; }

        public int? MinValue { get; }

        public Guid QuestionId { get; }

        public string QuestionText { get; }

        public QuestionType QuestionType { get; }

        public bool Required { get; }

        [RequiredIf(nameof(Required))]
        public string Value { get; set; }

        public bool ValueBoolean
        {
            get
            {
                bool.TryParse(Value, out bool boolValue);
                return boolValue;
            }
            set => Value = value.ToString();
        }

        public int ValueNumber
        {
            get
            {
                int.TryParse(Value, out int intValue);
                return intValue;
            }
            set => Value = value.ToString();
        }
    }
}
