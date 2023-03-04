using NoteMapper.Core.Guitars;

namespace NoteMapper.Services.Web.ViewModels.NoteMap
{
    public class NoteMapFretViewModel
    {
        private readonly HashSet<string> _availableModifiers = new();
        private readonly List<NoteMapNotesViewModel> _permutations = new();

        public NoteMapFretViewModel(int fret)
        {
            Fret = fret;
        }

        public IReadOnlyCollection<string> AvailableModifiers => _availableModifiers;

        public int Fret { get; }

        public IReadOnlyCollection<NoteMapNotesViewModel> Permutations => _permutations;        

        public NoteMapNotesViewModel? SelectedPermutation { get; private set; }

        public void AddPermutation(NoteMapNotesViewModel permutation)
        {
            _permutations.Add(permutation);

            foreach (NoteMapNoteViewModel? note in permutation.Notes)
            {
                if (note?.Modifier == null || _availableModifiers.Contains(note.Modifier))
                {
                    continue;
                }

                _availableModifiers.Add(note.Modifier);
            }

            if (SelectedPermutation == null)
            {
                SelectedPermutation = permutation;
            }
        }

        public void SetSelectedPermutation(NoteMapNotesViewModel permutation)
        {
            if (Permutations.Contains(permutation))
            {
                SelectedPermutation = permutation;
            }
        }
    }
}
