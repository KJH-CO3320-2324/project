using NoteMapper.Core.MusicTheory;
using NoteMapper.Core.NoteMap;

namespace NoteMapper.Services.Web.ViewModels.NoteMap
{
    public class NoteMapCriteriaViewModel
    {
        public const NoteMapMode DefaultMode = NoteMapMode.Combinations;
        public const NoteCollectionType DefaultType = NoteCollectionType.Chord;

        public HashSet<int> CustomNotes { get; set; } = new();

        public string? InstrumentId { get; set; }

        public NoteMapMode Mode { get; set; } = DefaultMode;

        public int NoteIndex { get; set; }

        public ScaleType ScaleType { get; set; }

        public NoteCollectionType Type { get; set; } = DefaultType;
    }
}
