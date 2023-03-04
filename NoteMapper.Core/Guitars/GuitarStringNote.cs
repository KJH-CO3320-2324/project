using NoteMapper.Core.MusicTheory;

namespace NoteMapper.Core.Guitars
{
    public class GuitarStringNote
    {        
        public GuitarStringNote(int fret, GuitarString @string, GuitarStringModifier? modifier)
        {            
            IReadOnlyCollection<GuitarStringModifier> modifiers = modifier != null 
                ? new[] { modifier } 
                : Array.Empty<GuitarStringModifier>();

            Fret = fret;
            Note = @string.NoteAt(fret, modifiers);
            Modifier = modifier;
            String = @string;
        }

        public int Fret { get; }

        public GuitarStringModifier? Modifier { get; }

        public Note Note { get; }

        public GuitarString String { get; }

        public override string ToString()
        {
            return $"S: {String} | N: {Note} | M: {Modifier} | F: {Fret}";
        }
    }
}
