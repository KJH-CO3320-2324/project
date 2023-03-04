using NoteMapper.Core.Extensions;

namespace NoteMapper.Core.MusicTheory
{
    public class Scale : NoteCollection
    {        
        private static readonly IReadOnlyDictionary<ScaleType, IReadOnlyCollection<byte>> KeyIntervals = CreateKeyIntervals();

        private static readonly IReadOnlyDictionary<ScaleType, string> KeyShortNames = CreateKeyShortNames();

        private Scale(IEnumerable<Note> notes, ScaleType type)
            : base(notes)
        {
            Type = type;
        }

        public override Scale Key => this;

        public ScaleType Type { get; }

        public static Scale Parse(int noteIndex, ScaleType scaleType)
        {
            Note note = new Note(noteIndex);
            
            IReadOnlyCollection<byte> intervals = GetIntervals(scaleType);
            IEnumerable<Note> notes = GetNotes(note, intervals);
            return new(notes, scaleType);
        }

        public static ScaleType ParseType(string type)
        {
            return KeyShortNames.FirstOrDefault(x => x.Value == type).Key;
        }

        public static Scale? TryParse(int noteIndex, ScaleType scaleType)
        {
            try
            {
                return Parse(noteIndex, scaleType);
            }
            catch
            {
                return default;
            }
        }

        public int GetInterval(Note note)
        {
            return IndexOf(note) + 1;
        }

        public IEnumerable<Note> NotesBetween(int start, int end)
        {
            while (start <= end)
            {
                int noteIndex = Note.GetNoteIndex(start);
                if (Contains(noteIndex))
                {
                    yield return new Note(start);
                }

                start++;
            }
        }

        public IEnumerable<Note> Overlapping(Scale other)
        {
            foreach (Note note in other)
            {
                if (Contains(note))
                {
                    yield return note;
                }
            }
        }

        private static IReadOnlyDictionary<ScaleType, IReadOnlyCollection<byte>> CreateKeyIntervals()
        {
            return new Dictionary<ScaleType, IReadOnlyCollection<byte>>
            {
                { ScaleType.DominantSeven, new byte[] { 2, 2, 1, 2, 2, 1 } },
                { ScaleType.Major, new byte[] { 2, 2, 1, 2, 2, 2 } },
                { ScaleType.Minor, new byte[] { 2, 1, 2, 2, 1, 2 } }
            }.AsReadOnly();
        }

        private static IReadOnlyDictionary<ScaleType, string> CreateKeyShortNames()
        {
            return Enum.GetValues<ScaleType>()
                .ToDictionary(x => x, x => x.ShortName())
                .AsReadOnly();
        }

        private static IReadOnlyCollection<byte> GetIntervals(ScaleType type)
        {
            if (KeyIntervals.ContainsKey(type))
            {
                return KeyIntervals[type];
            }

            if (type.ToString().StartsWith(ScaleType.Major.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return KeyIntervals[ScaleType.Major];
            }

            if (type.ToString().StartsWith(ScaleType.Minor.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return KeyIntervals[ScaleType.Minor];
            }

            return Array.Empty<byte>();
        }

        private static IEnumerable<Note> GetNotes(Note startingNote, IReadOnlyCollection<byte> intervals)
        {
            yield return startingNote;

            foreach (byte interval in intervals)
            {
                yield return startingNote = startingNote.Next(interval);
            }
        }

        private static ScaleType ParseScaleType(string type)
        {
            if (Enum.TryParse(type, out ScaleType parsed) && Enum.IsDefined(parsed))
            {
                return parsed;
            }

            if (KeyShortNames.Any(x => string.Equals(type, x.Value, StringComparison.InvariantCultureIgnoreCase)))
            {
                return KeyShortNames
                    .First(x => string.Equals(type, x.Value, StringComparison.InvariantCultureIgnoreCase))
                    .Key;
            }

            return default;
        }
    }
}
