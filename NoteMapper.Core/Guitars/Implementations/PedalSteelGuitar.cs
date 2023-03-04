namespace NoteMapper.Core.Guitars.Implementations
{
    public class PedalSteelGuitar : GuitarBase
    {
        private PedalSteelGuitar(string id, string name, IEnumerable<GuitarString> strings,
            GuitarStringModifierCollection modifiers)
            : base(modifiers)
        {
            Id = id;
            Name = name;
            Strings = strings.ToArray();
        }

        public override string Id { get; }

        public override string Name { get; }

        public override IReadOnlyCollection<GuitarString> Strings { get; }

        public override GuitarType Type => GuitarType.PedalSteelGuitar;

        public static PedalSteelGuitar Custom(string id, string name, PedalSteelGuitarConfig config)
        {
            List<GuitarStringModifier> modifiers = new();
            foreach (string m in config.Modifiers)
            {
                GuitarStringModifier modifier = GuitarStringModifier.Parse(m);
                modifiers.Add(modifier);
            }

            List<GuitarString> strings = new();
            for (int i = 0; i < config.Strings.Count; i++)
            {
                GuitarString @string = GuitarString.Parse(i, config.Strings.ElementAt(i), modifiers);
                strings.Add(@string);
            }

            GuitarStringModifierCollection modifierCollection = new(modifiers,
                config.MutuallyExclusiveModifiers);
            return new PedalSteelGuitar(id, name, strings, modifierCollection);
        }
    }
}
