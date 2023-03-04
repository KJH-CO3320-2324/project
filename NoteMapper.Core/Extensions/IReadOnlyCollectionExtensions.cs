namespace NoteMapper.Core.Extensions
{
    public static class IReadOnlyCollectionExtensions
    {
        public static int IndexOf<T>(this IReadOnlyCollection<T> collection, T value, IEqualityComparer<T>? comparer = null)
        {
            if (comparer == null)
            {
                comparer = EqualityComparer<T>.Default;
            }

            for (int i = 0; i < collection.Count; i++)
            {
                if (comparer.Equals(value, collection.ElementAt(i)))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
