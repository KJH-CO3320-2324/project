using System.Collections;
using NoteMapper.Core.Extensions;

namespace NoteMapper.Core.Permutations
{
    public class Permutation : IEnumerable<bool>
    {
        public Permutation(IReadOnlyCollection<bool> values)
        {
            Values = values;
            Count = values.Count;
        }        

        public int Count { get; }

        private IReadOnlyCollection<bool> Values { get; }

        public static IReadOnlyCollection<Permutation> GetPermutations(int numberOfOptions)
        {
            int numberOfPermutations = (int)Math.Pow(2, numberOfOptions);

            Permutation[] permutations = new Permutation[numberOfPermutations];

            for (int i = 0; i < numberOfPermutations; i++)
            {
                BitArray bitArray = new(new[] { i });                
                bool[] array = bitArray.ToArray(numberOfOptions);
                
                Permutation permutation = new(array);
                permutations[i] = permutation;
            }

            return permutations;
        }

        public static Permutation Parse(string bits)
        {
            bool[] array = new bool[bits.Length];
            for (int i = 0; i < bits.Length; i++)
            {
                array[i] = bits[i] == '1';
            }

            return new Permutation(array);
        }

        public bool Get(int i)
        {
            return i >=0 && i < Values.Count 
                ? Values.ElementAt(i)
                : false;
        }

        public IEnumerator<bool> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override int GetHashCode()
        {
            return ToBitArray().ToInt();
        }

        /// <summary>
        /// Returns true if the given permutation is a subset of the current permutation
        /// </summary>
        public bool Contains(Permutation other)
        {
            // The other bit array is a subset of this bit array if THIS | OTHER = THIS
            BitArray result = ToBitArray()
                .Or(other.ToBitArray());

            for (int i = 0; i < Values.Count; i++)
            {
                if (result.Get(i) != Values.ElementAt(i))
                {
                    return false;
                }
            }

            return true;
        }

        private BitArray ToBitArray()
        {
            return new BitArray(Values.ToArray());
        }
    }
}
