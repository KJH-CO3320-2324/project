namespace NoteMapper.Data.Core.Questionnaires
{
    public class Questionnaire
    {
        public Questionnaire(Guid questionnaireId, string name, DateTime? expiresUtc,
            bool active, string linkText, string introText)
        {
            Active = active;
            ExpiresUtc = expiresUtc;
            IntroText = introText;
            LinkText = linkText;
            Name = name;
            QuestionnaireId = questionnaireId;
        }

        public bool Active { get; }

        public DateTime? ExpiresUtc { get; }

        public string IntroText { get; }

        public string LinkText { get; }

        public string Name { get; }

        public Guid QuestionnaireId { get; }
    }
}
