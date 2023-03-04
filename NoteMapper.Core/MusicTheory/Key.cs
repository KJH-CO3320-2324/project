namespace NoteMapper.Core.MusicTheory
{
    public class Key
    {
        public Key(string shortName, string name)
        {
            Name = name;
            ShortName = shortName;
        }

        public string Name { get; }

        public string ShortName { get; }
    }
}
