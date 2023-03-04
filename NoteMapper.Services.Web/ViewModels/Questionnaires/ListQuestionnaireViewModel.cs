using NoteMapper.Data.Core.Questionnaires;

namespace NoteMapper.Services.Web.ViewModels.Questionnaires
{
    public class ListQuestionnaireViewModel
    {
        public ListQuestionnaireViewModel(Questionnaire questionnaire, int respondents)
        {
            Active = questionnaire.Active;
            ExpiresUtc = questionnaire.ExpiresUtc;
            Id = questionnaire.QuestionnaireId;
            Name = questionnaire.Name;
            Respondents = respondents;
        }

        public bool Active { get; }

        public DateTime? ExpiresUtc { get; }

        public Guid Id { get; }

        public string Name { get; }

        public int Respondents { get; }
    }
}
