using NoteMapper.Core;

namespace NoteMapper.Web.Blazor.Models
{
    public enum FeedbackType
    {
        None,
        Success,
        Danger
    }

    public class FeedbackViewModel
    {
        public string Message { get; set; } = "";

        public FeedbackType Type { get; set; } = FeedbackType.None;

        public static FeedbackViewModel FromServiceResult(ServiceResult result)
        {
            return new FeedbackViewModel
            {
                Message = result.Message ?? "",
                Type = result.Success ? FeedbackType.Success : FeedbackType.Danger
            };
        }
    }
}
