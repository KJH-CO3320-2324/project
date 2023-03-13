using NoteMapper.Core.MusicTheory;

namespace NoteMapper.Core.Tests
{
    public static class ScaleTests
    {
        [TestCase(9, ExpectedResult = "A,B,C#,D,E,F#,G#")]
        [TestCase(0, ExpectedResult = "C,D,E,F,G,A,B")]
        [TestCase(4, ExpectedResult = "E,F#,G#,A,B,C#,D#")]
        [TestCase(5, ExpectedResult = "F,G,A,A#,C,D,E")]
        public static string Major(int noteIndex)
        {
            Scale scale = Scale.Parse(noteIndex, ScaleType.Major);
            return string.Join(",", scale.Select(x => x.GetName(AccidentalType.Sharp)));
        }

        [TestCase(0, ExpectedResult = "C,D,D#,F,G,G#,A#")]        
        public static string Minor(int noteIndex)
        {
            Scale scale = Scale.Parse(noteIndex, ScaleType.Minor);
            return string.Join(",", scale.Select(x => x.GetName(AccidentalType.Sharp)));
        }

        [TestCase(0, ExpectedResult = "C0,D0,E0,F0,G0,A0,B0,C1,D1,E1,F1,G1,A1,B1,C2")]
        public static string NotesBetween(int noteIndex)
        {
            Scale scale = Scale.Parse(noteIndex, ScaleType.Major);
            IEnumerable<Note>? notes = scale?.NotesBetween(0, 24) ?? Enumerable.Empty<Note>();
            return string.Join(",", notes.Select(x => x.ToString()));
        }
    }
}