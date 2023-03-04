using System.ComponentModel.DataAnnotations;
using NoteMapper.Data.Core.Questionnaires;

namespace NoteMapper.Services.Web.ViewModels.Questionnaires
{
    public class EditQuestionnaireViewModel
    {
        public EditQuestionnaireViewModel(Questionnaire questionnaire, 
            IEnumerable<QuestionnaireQuestion> questions)
        {
            Active = questionnaire.Active;
            ExpiresUtc = questionnaire.ExpiresUtc;
            IntroText = questionnaire.IntroText;
            LinkText = questionnaire.LinkText;
            Name = questionnaire.Name;

            Questions.AddRange(questions.Select(x => new EditQuestionViewModel(x)));

            TypeOptions = Enum.GetValues<QuestionType>()
                .Where(x => x != QuestionType.None)
                .Select(x => new KeyValuePair<QuestionType, string>(x, x.ToString()))
                .ToArray();
        }

        public bool Active { get; set; }

        public DateTime? ExpiresUtc { get; set; }

        [Required]
        public string IntroText { get; set; }

        [Required]
        public string LinkText { get; set; }

        [Required]
        public string Name { get; set; }

        [ValidateComplexType]
        public List<EditQuestionViewModel> Questions { get; } = new();

        public IReadOnlyCollection<KeyValuePair<QuestionType, string>> TypeOptions { get; }

        public void AddQuestion()
        {
            Questions.Add(new EditQuestionViewModel());
        }

        public void RemoveQuestion(EditQuestionViewModel question)
        {
            Questions.Remove(question);
        }
    }
}
