namespace NoteMapper.Services.Web.ViewModels.NoteMap
{
    public class NoteMapViewModel
    {
        private readonly List<NoteMapFretViewModel> _frets = new();

        public IReadOnlyCollection<NoteMapFretViewModel> Frets => _frets;

        public void AddFret(NoteMapFretViewModel fret)
        {
            _frets.Add(fret);            
        }
    }
}
