using System.Text.RegularExpressions;
using NoteMapper.Core.MusicTheory;

namespace NoteMapper.Core.Guitars
{
    public class GuitarString
    {
        private static Regex _stringRegex = new(@"^n=(?<noteindex>\d+)\|o=(?<octaveindex>\d+)\|f=(?<startfret>\d+)\-(?<endfret>\d+)(|)?$", 
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public GuitarString(int index, Note openNote, int fret, IEnumerable<GuitarStringModifier> modifiers)
        {
            Index = index;
            Frets = fret;

            Modifiers = modifiers.ToArray();           
            OpenNote = openNote;
        }

        public int Frets { get; }

        public int Index { get; }        

        public Note OpenNote { get; }

        private IReadOnlyCollection<GuitarStringModifier> Modifiers { get; }

        public static GuitarString Parse(int index, string s, IReadOnlyCollection<GuitarStringModifier> modifiers)
        {
            Match match = _stringRegex.Match(s);
            if (!match.Success)
            {
                throw new ArgumentException("Invalid format", nameof(s));
            }

            int noteIndex = int.Parse(match.Groups["noteindex"].Value);
            int octaveIndex = int.Parse(match.Groups["octaveindex"].Value);
            int startFret = int.Parse(match.Groups["startfret"].Value);
            int endFret = int.Parse(match.Groups["endfret"].Value);

            Note openNote = new Note(noteIndex, octaveIndex);
            return new GuitarString(index, openNote, endFret, modifiers.Where(x => x.IsFor(index)));
        }

        public bool HasModifier(GuitarStringModifier modifier)
        {
            return Modifiers.Contains(modifier);
        }

        public Note NoteAt(int fret, IReadOnlyCollection<GuitarStringModifier> enabledModifiers)
        {
            if (fret < 0 || 
                fret > Frets)
            {
                throw new ArgumentOutOfRangeException(nameof(fret));
            }

            enabledModifiers = enabledModifiers
                .Where(x => HasModifier(x)).ToArray();

            int modifierOffset = enabledModifiers.Count > 0 ? enabledModifiers.Sum(x => x.GetOffset(this)) : 0;

            return OpenNote
                .Next(fret)
                .Next(modifierOffset);
        }

        public override string ToString()
        {
            return OpenNote.ToString();
        }
    }
}
