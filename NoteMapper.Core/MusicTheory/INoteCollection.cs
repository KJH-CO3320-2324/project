namespace NoteMapper.Core.MusicTheory
{
    public interface INoteCollection : IReadOnlyCollection<Note>
    {
        Scale Key { get; }

        NoteCollectionType Type { get; }

        bool Contains(Note note);

        int IndexOf(Note note);
    }
}
