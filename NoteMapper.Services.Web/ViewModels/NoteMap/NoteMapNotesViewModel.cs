using NoteMapper.Core.Guitars;
using NoteMapper.Core.MusicTheory;

namespace NoteMapper.Services.Web.ViewModels.NoteMap
{
    public class NoteMapNotesViewModel
    {
        public NoteMapNotesViewModel(IReadOnlyCollection<GuitarStringNote?> notes, Scale key)
        {
            Notes = notes
                .Select(x => x != null ? new NoteMapNoteViewModel(x, key) : null)
                .ToArray();
        }

        public IReadOnlyCollection<NoteMapNoteViewModel?> Notes { get; }        

        public bool HasModifier(string name)
        {
            return Notes.Any(x => 
                string.Equals(x?.Modifier, name, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
