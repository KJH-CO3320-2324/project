namespace NoteMapper.Core.MusicTheory
{
    public class Chord : ScaleNoteCollection
    {
        private Chord(Scale scale, IEnumerable<int> noteIndexes)
            : base(NoteCollectionType.Chord, scale, noteIndexes)
        {
        }

        public static Chord Parse(int noteIndex, ScaleType scaleType)
        {
            Scale scale = Scale.Parse(noteIndex, scaleType);

            IEnumerable<int> notes = GetChordNotes(scale.ScaleType);

            return new Chord(scale, notes);
        }

        private static IEnumerable<int> GetChordNotes(ScaleType type)
        {
            switch (type)
            {
                case ScaleType.Major:
                case ScaleType.Minor:
                    return new[] { 0, 2, 4 };
                case ScaleType.DominantSeven:
                case ScaleType.MajorSeven:
                case ScaleType.MinorSeven:
                    return GetChordNotes(ScaleType.Major).Append(6);
                default:
                    return Enumerable.Empty<int>();
            }
        }
    }
}
