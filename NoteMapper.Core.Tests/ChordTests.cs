using NoteMapper.Core.MusicTheory;

namespace NoteMapper.Core.Tests
{
    public static class ChordTests
    {
        [TestCase(0, ScaleType.Major, ExpectedResult = "C,E,G")]
        [TestCase(4, ScaleType.Major, ExpectedResult = "E,G#,B")]
        [TestCase(0, ScaleType.Minor, ExpectedResult = "C,D#,G")]
        [TestCase(4, ScaleType.Minor, ExpectedResult = "E,G,B")]
        [TestCase(0, ScaleType.MajorSeven, ExpectedResult = "C,E,G,B")]
        [TestCase(4, ScaleType.MajorSeven, ExpectedResult = "E,G#,B,D#")]
        [TestCase(0, ScaleType.MinorSeven, ExpectedResult = "C,D#,G,A#")]
        [TestCase(4, ScaleType.MinorSeven, ExpectedResult = "E,G,B,D")]
        [TestCase(0, ScaleType.DominantSeven, ExpectedResult = "C,E,G,A#")]
        [TestCase(4, ScaleType.DominantSeven, ExpectedResult = "E,G#,B,D")]
        public static string Parse(int noteIndex, ScaleType scaleType)
        {            
            Chord chord = Chord.Parse(noteIndex, scaleType);
            return string.Join(",", chord.Select(x => x.GetName(AccidentalType.Sharp)));
        }        
    }
}
