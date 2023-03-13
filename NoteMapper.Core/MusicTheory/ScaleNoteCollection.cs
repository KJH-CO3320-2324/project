namespace NoteMapper.Core.MusicTheory
{
    public class ScaleNoteCollection : NoteCollection
    {
        public ScaleNoteCollection(NoteCollectionType type, Scale scale, IEnumerable<int> noteIndexes) 
            : base(type, noteIndexes.Select(x => scale.ElementAt(x % scale.Count)))
        {
            Key = scale;
        }

        public override Scale Key { get; }
    }
}
