using System.ComponentModel.DataAnnotations;
using NoteMapper.Data.Core.Questionnaires;

namespace NoteMapper.Services.Web.ViewModels.Questionnaires
{
    public class QuestionnaireResponsesViewModel
    {
        public QuestionnaireResponsesViewModel(Questionnaire questionnaire,
            IReadOnlyCollection<QuestionnaireResponseViewModel> responses)
        {
            IntroText = questionnaire.IntroText;
            QuestionnaireId = questionnaire.QuestionnaireId;
            Responses = responses.ToArray();
        }

        public string IntroText { get; }

        public Guid QuestionnaireId { get; }

        [ValidateComplexType]
        public QuestionnaireResponseViewModel[] Responses { get; }
    }
}
