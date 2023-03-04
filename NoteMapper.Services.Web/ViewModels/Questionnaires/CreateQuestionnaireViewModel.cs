using NoteMapper.Data.Core.Questionnaires;

namespace NoteMapper.Services.Web.ViewModels.Questionnaires
{
    public class CreateQuestionnaireViewModel : EditQuestionnaireViewModel
    {
        public CreateQuestionnaireViewModel()
            : base(new Questionnaire(Guid.Empty, "", null, false, "", ""), 
                  Enumerable.Empty<QuestionnaireQuestion>())
        {
        }
    }
}
