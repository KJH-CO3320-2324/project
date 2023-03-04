using System.Text.RegularExpressions;
using NoteMapper.Core.Permutations;

namespace NoteMapper.Core.Guitars
{
    public class GuitarStringModifier
    {
        private static Regex _parseRegex = new(@"^(?<type>\w*?)\|(?<name>.+)\|(?<modifiers>.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex _parseModifierRegex = new(@"^(?<string>\d+)(?<offset>(\+|\-)\d+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);        

        private GuitarStringModifier(string type, string name, IDictionary<int, int> offsets)
        {
            Name = name;
            Offsets = offsets.AsReadOnly();
            Type = type;
        }

        public string Name { get; }

        private ICollection<GuitarStringModifier> MutuallyExclusiveModifiers { get; } = new List<GuitarStringModifier>();

        private IReadOnlyDictionary<int, int> Offsets { get; }        

        public string Type { get; }

        public static IDictionary<int, IReadOnlyCollection<GuitarStringModifier>> GetPermutations(
            IReadOnlyCollection<GuitarStringModifier> modifiers)
        {
            IDictionary<int, IReadOnlyCollection<GuitarStringModifier>> modifierPermutations =
                    new Dictionary<int, IReadOnlyCollection<GuitarStringModifier>>();

            foreach (Permutation permutation in Permutation.GetPermutations(modifiers.Count))
            {
                IReadOnlyCollection<GuitarStringModifier> permutationModifiers = modifiers
                    .Where((modifier, i) => permutation.Get(i))
                    .ToArray();


            }

            return modifierPermutations;
        }

        public static GuitarStringModifier Parse(string s)
        {
            Match match = _parseRegex.Match(s);
            if (!match.Success)
            {
                throw new ArgumentException("Invalid format", nameof(s));
            }

            string type = match.Groups["type"].Value;
            string name = match.Groups["name"].Value;
            string modifierString = match.Groups["modifiers"].Value;
            string[] modifierStrings = modifierString.Split(',');            

            IDictionary<int, int> offsets = new Dictionary<int, int>();

            foreach (string m in modifierStrings)
            {
                if (string.IsNullOrEmpty(m))
                {
                    continue;
                }

                Match modifierMatch = _parseModifierRegex.Match(m);
                if (!modifierMatch.Success)
                {
                    throw new ArgumentException("Invalid format", nameof(s));
                }

                int stringIndex = int.Parse(modifierMatch.Groups["string"].Value);
                int offset = int.Parse(modifierMatch.Groups["offset"].Value);

                offsets.Add(stringIndex, offset);
            }

            return new GuitarStringModifier(type, name, offsets);
        }

        public bool CanBeUsedWith(IEnumerable<GuitarStringModifier> modifiers)
        {
            foreach (GuitarStringModifier mutuallyExclusiveModifier in MutuallyExclusiveModifiers)
            {
                if (modifiers.Contains(mutuallyExclusiveModifier))
                {
                    return false;
                }
            }

            foreach (GuitarStringModifier modifier in modifiers)
            {
                if (MutuallyExclusiveModifiers.Contains(modifier))
                {
                    return false;
                }
            }

            return true;
        }

        public int GetOffset(GuitarString @string)
        {
            return GetOffset(@string.Index);
        }

        public int GetOffset(int stringIndex)
        {
            return Offsets.ContainsKey(stringIndex)
                ? Offsets[stringIndex]
                : 0;
        }

        public bool IsFor(int stringIndex)
        {
            return Offsets.ContainsKey(stringIndex);
        }

        public bool IsMutuallyExclusiveWith(GuitarStringModifier other)
        {
            return MutuallyExclusiveModifiers.Contains(other);
        }

        public GuitarStringModifier SetMutuallyExclusiveWith(GuitarStringModifier other)
        {
            if (other == this)
            {
                return this;
            }

            if (MutuallyExclusiveModifiers.Contains(other))
            {
                return this;
            }

            MutuallyExclusiveModifiers.Add(other);
            other.SetMutuallyExclusiveWith(this);
            return this;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
