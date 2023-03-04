using NoteMapper.Core.Extensions;
using NoteMapper.Core.MusicTheory;

namespace NoteMapper.Services
{
    public class MusicTheoryService : IMusicTheoryService
    {
        private static readonly IReadOnlyCollection<ScaleType> _keyTypes = Enum.GetValues<ScaleType>()
            .ToArray();

        public IReadOnlyCollection<(int Natural, int? Sharp)> GetNaturalNoteIndexesWithSharps()
        {
            IReadOnlyCollection<int> noteIndexes = GetNoteIndexes();

            List<(int Natural, int? Sharp)> naturalNoteIndexesWithSharps = new();

            for (int i = 0; i < noteIndexes.Count; i++)
            {
                int natural = noteIndexes.ElementAt(i);

                int? sharp = i < noteIndexes.Count - 1 && !Note.IsNatural(noteIndexes.ElementAt(i + 1))
                    ? noteIndexes.ElementAt(++i)
                    : null;

                naturalNoteIndexesWithSharps.Add((natural, sharp));
            }

            return naturalNoteIndexesWithSharps;
        }

        public IReadOnlyCollection<int> GetNoteIndexes()
        {
            return Note.GetNoteIndexes();
        }

        public IReadOnlyCollection<ScaleType> GetScaleTypes()
        {
            return _keyTypes;
        }
    }
}
