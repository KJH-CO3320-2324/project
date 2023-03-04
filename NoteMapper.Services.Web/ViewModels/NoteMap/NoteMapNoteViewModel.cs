using NoteMapper.Core.Guitars;
using NoteMapper.Core.MusicTheory;

namespace NoteMapper.Services.Web.ViewModels.NoteMap
{
    public class NoteMapNoteViewModel
    {
        public NoteMapNoteViewModel(GuitarStringNote note, Scale key)
        {
            Interval = key.GetInterval(note.Note);
            Modifier = note.Modifier?.Name;
            NoteIndex = note.Note.NoteIndex;
        }

        public int Interval { get; set; }

        public string? Modifier { get; set; }

        public int NoteIndex { get; set; }
    }
}
