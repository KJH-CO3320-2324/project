using NoteMapper.Core.Permutations;

namespace NoteMapper.Core.Tests.Permutations
{
    public static class PermutationTests
    {
        [TestCase("10101", "10001", ExpectedResult = true)]
        [TestCase("10101", "11001", ExpectedResult = false)]
        public static bool Contains(string bits, string otherBits)
        {
            Permutation permutation = Permutation.Parse(bits);
            Permutation other = Permutation.Parse(otherBits);

            return permutation.Contains(other);
        }

        [Test]
        public static void GetPermutations()
        {
            IReadOnlyCollection<Permutation> permutations = Permutation.GetPermutations(3);

            IReadOnlyCollection<IReadOnlyCollection<bool>> expected = new[]
            {
                new[] { false, false, false },
                new[] { true, false, false },
                new[] { false, true, false },
                new[] { true, true, false },
                new[] { false, false, true },
                new[] { true, false, true },
                new[] { false, true, true },
                new[] { true, true, true }
            };

            CollectionAssert.AreEqual(expected, permutations);
        }

        [Test]
        public static void Parse()
        {
            string bits = "10101";
            Permutation permutation = Permutation.Parse(bits);
            bool[] expected = new[]
            {
                true, false, true, false, true
            };

            bool[] actual = permutation.ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
