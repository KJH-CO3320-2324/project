namespace NoteMapper.Services.Web.ViewModels
{
    public class KeyListItemViewModel
    {
        public KeyListItemViewModel(string shortName, string name)
        {
            ShortName = shortName;
            Name = name;
        }            

        public string Name { get; }

        public string ShortName { get; }
    }
}
