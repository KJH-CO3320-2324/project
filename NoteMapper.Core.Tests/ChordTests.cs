using NoteMapper.Core.MusicTheory;

namespace NoteMapper.Core.Tests
{
    public static class ChordTests
    {
        [TestCase(0, "", ExpectedResult = "C,E,G")]
        [TestCase(4, "", ExpectedResult = "E,G#,B")]
        [TestCase(0, "m", ExpectedResult = "C,D#,G")]
        [TestCase(4, "m", ExpectedResult = "E,G,B")]
        [TestCase(0, "maj7", ExpectedResult = "C,E,G,B")]
        [TestCase(4, "maj7", ExpectedResult = "E,G#,B,D#")]
        [TestCase(0, "m7", ExpectedResult = "C,D#,G,A#")]
        [TestCase(4, "m7", ExpectedResult = "E,G,B,D")]
        [TestCase(0, "7", ExpectedResult = "C,E,G,A#")]
        [TestCase(4, "7", ExpectedResult = "E,G#,B,D")]
        public static string Parse(int noteIndex, string key)
        {            
            Chord chord = Chord.Parse(noteIndex, key);
            return string.Join(",", chord.Select(x => x.GetName(AccidentalType.Sharp)));
        }        
    }
}
