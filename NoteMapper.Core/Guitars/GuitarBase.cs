using NoteMapper.Core.Extensions;
using NoteMapper.Core.MusicTheory;
using NoteMapper.Core.Permutations;

namespace NoteMapper.Core.Guitars
{
    public abstract class GuitarBase
    {
        private readonly Lazy<int> _frets;

        protected GuitarBase(GuitarStringModifierCollection modifiers)
        {
            Modifiers = modifiers;

            _frets = new Lazy<int>(() => Strings.Max(x => x.Frets));
        }

        public int Frets => _frets.Value;

        public abstract string Id { get; }

        public int? MaxStringPairDistance { get; }

        public abstract string Name { get; }

        public GuitarStringModifierCollection Modifiers { get; }

        public IReadOnlyCollection<string> ModifierTypes => Type.ModifierTypes().ToArray();

        public abstract IReadOnlyCollection<GuitarString> Strings { get; }

        public abstract GuitarType Type { get; }                

        public IReadOnlyCollection<IReadOnlyCollection<GuitarStringNote?>> GetPermutations(StringPermutationOptions options)
        {
            List<IReadOnlyCollection<GuitarStringNote?>> notePermutations = new();
            
            // Store an index of the modifier permutations that were used to avoid duplicating the
            // permutations containing redundant modifiers
            HashSet<int> usedPermutations = new();

            foreach (IReadOnlyCollection<GuitarStringModifier> modifierPermutation in Modifiers.GetPermutations())
            {
                IReadOnlyCollection<GuitarStringNote?> stringNotes = GetNotes(modifierPermutation, options)
                    .ToArray();

                if (stringNotes.Count == 0)
                {
                    continue;
                }

                Permutation permutation = GetUsedModifierPermutation(stringNotes);
                int permutationHashCode = permutation.GetHashCode();
                if (usedPermutations.Contains(permutationHashCode))
                {
                    // this combination of applied modifiers has already been used
                    // do not use this permutation as it is redundant
                    continue;
                }

                usedPermutations.Add(permutationHashCode);

                notePermutations.Add(stringNotes);
            }

            return notePermutations;
        }

        public IEnumerable<GuitarStringNote?> GetNotes(IEnumerable<string> modifierNames,
            StringPermutationOptions options)
        {
            IEnumerable<GuitarStringModifier> modifiers = Modifiers
                .Where(x => modifierNames.Contains(x.Name));
            return GetNotes(modifiers, options);
        }

        public IEnumerable<GuitarStringNote?> GetNotes(IEnumerable<GuitarStringModifier> modifiers,
            StringPermutationOptions options)
        {
            // the composition of the note, string, and possible modifier
            List<GuitarStringNote?> stringNotes = new();            
            // which notes are being played in this permutation
            HashSet<int> usedNotes = new();

            for (int i = 0; i < Strings.Count; i++)
            {
                // the current string
                GuitarString @string = Strings.ElementAt(i);
                // get the first modifier from this permutation that applies to the current string
                // it is assumed that modifiers are declared in order of precedence
                GuitarStringModifier? modifier = modifiers
                    .FirstOrDefault(x => @string.HasModifier(x));
                // the note being played on this string
                Note note = @string.NoteAt(options.Fret,
                    modifier != null ? new[] { modifier } : Array.Empty<GuitarStringModifier>());
                if (options.Notes.All(x => x.NoteIndex != note.NoteIndex))
                {
                    // the note isn't in the current set of notes
                    stringNotes.Add(null);
                    continue;
                }

                usedNotes.Add(note.NoteIndex);

                GuitarStringNote stringNote = new(options.Fret, @string, modifier);
                stringNotes.Add(stringNote);
            }

            if (options.Notes.Count(x => usedNotes.Contains(x.NoteIndex)) < options.Threshold)
            {
                // not enough notes were found, do not use this permutation
                return Enumerable.Empty<GuitarStringNote?>();
            }

            return stringNotes;
        }

        public IEnumerable<GuitarStringModifier> AvailableModifiers(INoteCollection possibleNotes, int fret)
        {
            foreach (GuitarStringModifier modifier in Modifiers)
            {
                foreach (GuitarString @string in Strings.Where(x => x.HasModifier(modifier)))
                {
                    Note note = @string.NoteAt(fret, new[] { modifier });
                    if (possibleNotes.Contains(note))
                    {
                        yield return modifier;
                        break;
                    }
                }
            }
        }

        private Permutation GetUsedModifierPermutation(IEnumerable<GuitarStringNote?> stringNotes)
        {
            bool[] usedModifiers = new bool[Modifiers.Count];
            foreach (GuitarStringNote? stringNote in stringNotes)
            {
                if (stringNote?.Modifier == null)
                {
                    continue;
                }

                int modifierIndex = Modifiers.IndexOf(stringNote.Modifier);
                usedModifiers[modifierIndex] = true;
            }

            Permutation permutation = new(usedModifiers);
            return permutation;
        }
    }
}
