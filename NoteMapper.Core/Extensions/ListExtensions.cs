namespace NoteMapper.Core.Extensions
{
    public static class ListExtensions
    {
        public static void MoveOne<T>(this List<T> list, T entry, int direction)
        {
            int index = list.IndexOf(entry);
            if (index < 0)
            {
                return;
            }

            if ((direction < 0 && index == 0) ||
                (direction > 0 && index == list.Count - 1))
            {
                return;
            }

            int swapIndex = direction < 0 ? index - 1 : index + 1;
            T swap = list[swapIndex];
            list[index] = swap;
            list[swapIndex] = entry;


        }

        public static void SetCount<T>(this List<T> list, int count, T defaultValue)
        {
            list.SetCount(count, () => defaultValue);         
        }

        public static void SetCount<T>(this List<T> list, int count, Func<T> getDefaultValue)
        {
            for (int i = list.Count - 1; i >= count; i--)
            {
                list.RemoveAt(i);
            }

            while (list.Count < count)
            {
                list.Add(getDefaultValue());
            }
        }        
    }
}
