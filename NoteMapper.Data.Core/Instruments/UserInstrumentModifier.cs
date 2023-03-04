namespace NoteMapper.Data.Core.Instruments
{
    public class UserInstrumentModifier
    {
        public List<string>? MutuallyExclusive { get; set; }

        public string Name { get; set; } = "";

        public List<ModifierOffset> Offsets { get; set; } = new();

        public string Type { get; set; } = "";
    }
}
