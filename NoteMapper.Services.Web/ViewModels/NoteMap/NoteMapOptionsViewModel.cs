using NoteMapper.Core.MusicTheory;
using NoteMapper.Core.NoteMap;

namespace NoteMapper.Services.Web.ViewModels.NoteMap
{
    public class NoteMapOptionsViewModel
    {
        public AccidentalType Accidental { get; set; }

        public IReadOnlyCollection<int> CustomNotes { get; set; } = Array.Empty<int>();

        public ScaleType ScaleType { get; set; } = ScaleType.Major;

        public NoteMapMode Mode { get; set; }

        public IReadOnlyCollection<string> Modifiers { get; set; } = Array.Empty<string>();

        public int NoteIndex { get; set; }

        public NoteCollectionType Type { get; set; }
    }
}
