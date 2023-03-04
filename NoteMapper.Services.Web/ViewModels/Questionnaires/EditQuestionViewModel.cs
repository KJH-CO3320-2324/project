using System.ComponentModel.DataAnnotations;
using NoteMapper.Data.Core.Questionnaires;

namespace NoteMapper.Services.Web.ViewModels.Questionnaires
{
    public class EditQuestionViewModel
    {
        public EditQuestionViewModel()
        {
            QuestionText = "";
            Type = QuestionType.LongText;
        }

        public EditQuestionViewModel(QuestionnaireQuestion question)
        {
            MaxValue = question.MaxValue;
            MinValue = question.MinValue;
            QuestionId = question.QuestionId;
            QuestionText = question.QuestionText;
            Required = question.Required;
            Type = question.Type;            
        }

        public int? MaxValue { get; set; }

        public int? MinValue { get; set; }

        public Guid? QuestionId { get; }

        [Display(Name = "Text")]
        [Required]
        public string QuestionText { get; set; }

        public bool Required { get; set; }

        public QuestionType Type { get; set; }        
    }
}
