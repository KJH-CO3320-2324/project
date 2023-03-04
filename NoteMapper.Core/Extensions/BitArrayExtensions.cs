using System.Collections;

namespace NoteMapper.Core.Extensions
{
    public static class BitArrayExtensions
    {
        public static bool[] ToArray(this BitArray bitArray, int length)
        {
            bool[] array = new bool[length];
            for (int i = 0; i < length; i++)
            {
                if (i > bitArray.Length - 1)
                {
                    break;
                }

                array[i] = bitArray[i];
            }

            return array;
        }

        public static int ToInt(this BitArray bitArray)
        {            
            int[] array = new int[1];
            bitArray.CopyTo(array, 0);
            return array[0];
        }
    }
}
