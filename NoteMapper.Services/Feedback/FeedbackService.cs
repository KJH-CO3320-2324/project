namespace NoteMapper.Services.Feedback
{
    public class FeedbackService : IFeedbackService
    {
        public event Action<FeedbackMessage>? OnNotify;

        public void Notify(FeedbackMessage result)
        {
            OnNotify?.Invoke(result);
        }
    }
}
