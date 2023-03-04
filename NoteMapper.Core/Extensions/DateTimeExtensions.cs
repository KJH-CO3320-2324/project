namespace NoteMapper.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToUtcDateString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
        }
    }
}
