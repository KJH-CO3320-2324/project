namespace NoteMapper.Core.MusicTheory
{
    public interface INoteCollection : IReadOnlyCollection<Note>
    {
        Scale Key { get; }

        bool Contains(Note note);

        int IndexOf(Note note);
    }
}
