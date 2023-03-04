using NoteMapper.Core;

namespace NoteMapper.Services.Feedback
{
    public class FeedbackMessage
    {
        public const int DefaultDisplaySeconds = 5;

        public FeedbackMessage(FeedbackType type, string message)
            : this(type, message, DefaultDisplaySeconds)
        {            
        }

        public FeedbackMessage(FeedbackType type, string message, int displayFor)
        {
            DisplayFor = displayFor;
            Message = message;
            Type = type;
        }

        public FeedbackMessage(ServiceResult result)
            : this(result.Success ? FeedbackType.Success : FeedbackType.Danger, result.Message ?? "")
        {
        }

        public int DisplayFor { get; }

        public string? Heading { get; set; }

        public string Message { get; }

        public FeedbackType Type { get; }
    }
}
