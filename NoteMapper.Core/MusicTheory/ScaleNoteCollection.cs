namespace NoteMapper.Core.MusicTheory
{
    public class ScaleNoteCollection : NoteCollection
    {
        public ScaleNoteCollection(Scale scale, IEnumerable<int> noteIndexes) 
            : base(noteIndexes.Select(x => scale.ElementAt(x % scale.Count)))
        {
            Key = scale;
        }

        public override Scale Key { get; }
    }
}
