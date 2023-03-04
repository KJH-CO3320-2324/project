using NoteMapper.Core.Guitars;
using NoteMapper.Core.MusicTheory;

namespace NoteMapper.Services.Web.ViewModels.NoteMap
{
    public class NoteMapCriteriaOptionsViewModel
    {
        public NoteMapCriteriaOptionsViewModel(IEnumerable<GuitarBase> defaultInstruments, 
            IEnumerable<GuitarBase> userInstruments,
            IReadOnlyCollection<(int Natural, int? Sharp)> noteIndexes,
            IReadOnlyCollection<ScaleType> scaleTypes)
        {
            DefaultInstruments = defaultInstruments.ToArray();
            UserInstruments = userInstruments.ToArray();

            NoteIndexes = noteIndexes;
            ScaleTypes = scaleTypes;            

            CustomNoteOptions = new KeyValuePair<int, string>[]
            {
                new KeyValuePair<int, string>(0, "i"),
                new KeyValuePair<int, string>(1, "ii"),
                new KeyValuePair<int, string>(2, "iii"),
                new KeyValuePair<int, string>(3, "iv"),
                new KeyValuePair<int, string>(4, "v"),
                new KeyValuePair<int, string>(5, "vi"),
                new KeyValuePair<int, string>(6, "vii")
            };

            TypeOptions = new[]
            {
                new KeyValuePair<string, string?>(NoteCollectionType.Chord.ToString(), null),
                new KeyValuePair<string, string?>(NoteCollectionType.Scale.ToString(), null),
                new KeyValuePair<string, string?>(NoteCollectionType.Custom.ToString(), null)
            };
        }        

        public IReadOnlyCollection<KeyValuePair<int, string>> CustomNoteOptions { get; }

        public IReadOnlyCollection<GuitarBase> DefaultInstruments { get; }

        public IReadOnlyCollection<(int Natural, int? Sharp)> NoteIndexes { get; }

        public IReadOnlyCollection<ScaleType> ScaleTypes { get; }

        public IReadOnlyCollection<KeyValuePair<string, string?>> TypeOptions { get; }

        public IReadOnlyCollection<GuitarBase> UserInstruments { get; }
    }
}
