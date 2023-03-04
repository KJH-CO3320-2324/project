using NoteMapper.Core.MusicTheory;

namespace NoteMapper.Services
{
    public interface IMusicTheoryService
    {
        IReadOnlyCollection<(int Natural, int? Sharp)> GetNaturalNoteIndexesWithSharps();

        IReadOnlyCollection<int> GetNoteIndexes();

        IReadOnlyCollection<ScaleType> GetScaleTypes();
    }
}
