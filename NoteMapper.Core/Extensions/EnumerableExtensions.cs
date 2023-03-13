namespace NoteMapper.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : class
        {
            foreach (T? element in source)
            {
                if (element == null)
                {
                    continue;
                }

                yield return element;
            }
        }
    }
}
