using NoteMapper.Core.MusicTheory;

namespace NoteMapper.Services.Web.ViewModels.NoteMap
{
    public class NoteMapDisplayPreferenceOptionsViewModel
    {
        public NoteMapDisplayPreferenceOptionsViewModel()
        {
            AccidentalOptions = new[]
            {
                GetAccidentalOption(AccidentalType.Sharp),
                GetAccidentalOption(AccidentalType.Flat)
            };
        }

        public IReadOnlyCollection<KeyValuePair<AccidentalType, string>> AccidentalOptions { get; }

        private static KeyValuePair<AccidentalType, string> GetAccidentalOption(AccidentalType accidental)
        {
            return new KeyValuePair<AccidentalType, string>(accidental, Accidental.ToString(accidental));
        }
    }
}
