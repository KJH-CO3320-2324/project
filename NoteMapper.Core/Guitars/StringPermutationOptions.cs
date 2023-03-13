using NoteMapper.Core.MusicTheory;

namespace NoteMapper.Core.Guitars
{
    public class StringPermutationOptions
    {
        public StringPermutationOptions(INoteCollection notes, int fret, int? threshold,
            int maxChordStringGap)
        {
            Fret = fret;
            MaxChordStringGap = maxChordStringGap;
            Notes = notes;
            Threshold = threshold ?? notes.Count;
        }

        public int Fret { get; }

        public int MaxChordStringGap { get; }

        public INoteCollection Notes { get; }        

        public int Threshold { get; }
    }
}
