using NoteMapper.Core.MusicTheory;

namespace NoteMapper.Core.Tests
{
    public static class NoteTests
    {
        [TestCase(-1, ExpectedResult = 11)]
        [TestCase(-2, ExpectedResult = 10)]
        [TestCase(-3, ExpectedResult = 9)]
        [TestCase(-12, ExpectedResult = 0)]
        public static int Constructor_IndexLessThanZero_SetsIndexToPositiveValue(int index)
        {
            Note note = new Note(index);
            return note.Index;
        }

        [TestCase(0, ExpectedResult = "C")]
        [TestCase(2, ExpectedResult = "D")]
        [TestCase(4, ExpectedResult = "E")]
        [TestCase(5, ExpectedResult = "F")]
        [TestCase(7, ExpectedResult = "G")]
        [TestCase(9, ExpectedResult = "A")]
        [TestCase(11, ExpectedResult = "B")]
        public static string GetName_NaturalNotes_ReturnsName(int noteIndex)
        {
            Note note = new Note(noteIndex);
            return note.GetName(AccidentalType.Sharp);
        }

        [TestCase(1, AccidentalType.Sharp, ExpectedResult = "C#")]
        [TestCase(1, AccidentalType.Flat, ExpectedResult = "D♭")]
        [TestCase(3, AccidentalType.Sharp, ExpectedResult = "D#")]
        [TestCase(3, AccidentalType.Flat, ExpectedResult = "E♭")]
        [TestCase(6, AccidentalType.Sharp, ExpectedResult = "F#")]
        [TestCase(6, AccidentalType.Flat, ExpectedResult = "G♭")]
        [TestCase(8, AccidentalType.Sharp, ExpectedResult = "G#")]
        [TestCase(8, AccidentalType.Flat, ExpectedResult = "A♭")]
        [TestCase(10, AccidentalType.Sharp, ExpectedResult = "A#")]
        [TestCase(10, AccidentalType.Flat, ExpectedResult = "B♭")]
        public static string GetName_AccidentalNotes_ReturnsName(int noteIndex, AccidentalType accidental)
        {
            Note note = new Note(noteIndex);
            return note.GetName(accidental);
        }

        [TestCase(0, 0, 0, ExpectedResult = "00")]
        [TestCase(0, 0, 12, ExpectedResult = "01")]
        [TestCase(0, 4, 5, ExpectedResult = "54")]
        public static string Next(int noteIndex, int octaveIndex, int offset)
        {
            Note note = new(noteIndex, octaveIndex);
            Note next = note.Next(offset);
            return $"{next.NoteIndex}{next.OctaveIndex}";
        }
    }
}
