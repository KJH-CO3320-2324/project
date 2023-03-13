namespace NoteMapper.Core.MusicTheory
{
    public class NoteCollectionOptions
    {
        public IReadOnlyCollection<int>? CustomNotes { get; set; }

        public int NoteIndex { get; set; } 
        
        public ScaleType ScaleType { get; set; } 
        
        public NoteCollectionType Type { get; set; }            
    }
}
