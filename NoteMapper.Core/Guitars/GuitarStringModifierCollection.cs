using System.Collections;
using NoteMapper.Core.Extensions;
using NoteMapper.Core.Permutations;

namespace NoteMapper.Core.Guitars
{
    public class GuitarStringModifierCollection : IReadOnlyCollection<GuitarStringModifier>
    {        
        private readonly IReadOnlyCollection<GuitarStringModifier> _modifiers;
        private readonly IReadOnlyCollection<KeyValuePair<string, string>> _mutuallyExclusive;
        private IReadOnlyCollection<IReadOnlyCollection<GuitarStringModifier>>? _permutations;

        public GuitarStringModifierCollection(IEnumerable<GuitarStringModifier> modifiers,
            IEnumerable<KeyValuePair<string, string>> mutuallyExclusive)
        {
            _modifiers = modifiers.ToArray();
            _mutuallyExclusive = mutuallyExclusive.ToArray();

            SetMutuallyExclusiveModifiers();
        }

        public int Count => _modifiers.Count;

        public IEnumerator<GuitarStringModifier> GetEnumerator()
        {
            return _modifiers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }        

        public IReadOnlyCollection<IReadOnlyCollection<GuitarStringModifier>> GetPermutations()
        {
            if (_permutations != null)
            {
                return _permutations;
            }            

            HashSet<Permutation> invalidPermutations = new();
            foreach (GuitarStringModifier modifier1 in this)
            {
                foreach (GuitarStringModifier modifier2 in this)
                {
                    if (modifier1 == modifier2)
                    {
                        continue;
                    }

                    if (!modifier1.IsMutuallyExclusiveWith(modifier2))
                    {
                        continue;
                    }

                    bool[] bits = new bool[_modifiers.Count];
                    bits[_modifiers.IndexOf(modifier1)] = true;
                    bits[_modifiers.IndexOf(modifier2)] = true;

                    invalidPermutations.Add(new Permutation(bits));
                }                
            }

            List<IReadOnlyCollection<GuitarStringModifier>> modifierPermutations = new();

            IReadOnlyCollection<Permutation> permutations = Permutation.GetPermutations(_modifiers.Count);
            foreach (Permutation permutation in permutations)
            {
                if (invalidPermutations.Any(x => permutation.Contains(x)))
                {
                    continue;
                }

                IReadOnlyCollection<GuitarStringModifier> permutationModifiers = _modifiers
                    .Where((modifier, i) => permutation.Get(i))
                    .ToArray();
                modifierPermutations.Add(permutationModifiers);
            }

            _permutations = modifierPermutations;

            return _permutations;
        }

        private void SetMutuallyExclusiveModifiers()
        {
            foreach (KeyValuePair<string, string> pair in _mutuallyExclusive)
            {
                GuitarStringModifier? modifier1 = _modifiers
                    .FirstOrDefault(x => string.Equals(x.Name, pair.Key, StringComparison.InvariantCultureIgnoreCase));
                GuitarStringModifier? modifier2 = _modifiers
                    .FirstOrDefault(x => string.Equals(x.Name, pair.Value, StringComparison.InvariantCultureIgnoreCase));

                if (modifier1 == null || modifier2 == null)
                {
                    continue;
                }

                modifier1.SetMutuallyExclusiveWith(modifier2);                
            }
        }
    }
}
