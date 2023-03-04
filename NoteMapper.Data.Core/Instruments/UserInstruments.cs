namespace NoteMapper.Data.Core.Instruments
{
    public class UserInstruments
    {
        public string Id { get; set; } = "";

        public IList<UserInstrument> Instruments { get; set; } = new List<UserInstrument>();
    }
}
