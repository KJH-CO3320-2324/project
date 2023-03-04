namespace NoteMapper.Core.Guitars
{
    public static class GuitarTypeExtensions
    {
        public static IEnumerable<string> ModifierTypes(this GuitarType type)
        {
            switch (type)
            {
                case GuitarType.PedalSteelGuitar:
                    return new[]
                    {
                        "Pedal",
                        "Lever"
                    };
                default:
                    return Array.Empty<string>();
            }
        }
    }
}
