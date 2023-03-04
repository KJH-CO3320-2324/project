namespace NoteMapper.Services.Feedback
{
    public interface IFeedbackService
    {
        event Action<FeedbackMessage> OnNotify;

        void Notify(FeedbackMessage result);
    }
}
