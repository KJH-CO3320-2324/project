using NoteMapper.Core.Guitars;
using NoteMapper.Core.Guitars.Implementations;
using NoteMapper.Data.Core.Instruments;

namespace NoteMapper.Services
{
    public class InstrumentFactory : IInstrumentFactory
    {
        public GuitarBase FromUserInstrument(UserInstrument userInstrument)
        {
            GuitarType type = userInstrument.Type;
            
            switch (type)
            {
                case GuitarType.PedalSteelGuitar:
                    List<string> modifiers = new();
                    List<KeyValuePair<string, string>> mutuallyExclusiveModifiers = new();
                    List<string> strings = new();

                    foreach (UserInstrumentModifier modifier in userInstrument.Modifiers)
                    {
                        List<int> offsets = new();
                        foreach (ModifierOffset offset in modifier.Offsets)
                        {
                            offsets.Add(offset.String);
                            offsets.Add(offset.Offset);
                        }

                        string modifierConfig = PedalSteelGuitarConfig.GetModifierConfig(modifier.Type, modifier.Name, offsets.ToArray());
                        modifiers.Add(modifierConfig);

                        if (modifier.MutuallyExclusive != null)
                        {
                            foreach (string otherModifier in modifier.MutuallyExclusive)
                            {
                                mutuallyExclusiveModifiers.Add(new KeyValuePair<string, string>(modifier.Name, otherModifier));
                            }
                        }
                    }

                    foreach (UserInstrumentString @string in userInstrument.Strings)
                    {
                        string stringConfig = PedalSteelGuitarConfig.GetStringConfig(@string.NoteIndex, @string.OctaveIndex, userInstrument.Frets);
                        strings.Add(stringConfig);
                    }

                    PedalSteelGuitarConfig config = new()
                    {
                        Modifiers = modifiers,
                        MutuallyExclusiveModifiers = mutuallyExclusiveModifiers,
                        Strings = strings
                    };
                    return PedalSteelGuitar.Custom(userInstrument.UserInstrumentId, userInstrument.Name, config);
                default:
                    throw new NotImplementedException();
            }
        }

        public UserInstrument ToUserInstrument(GuitarBase instrument)
        {
            return new UserInstrument
            {
                Frets = 12,
                Modifiers = instrument.Modifiers.Select(x =>
                {
                    IEnumerable<ModifierOffset> offsets = instrument.Modifiers
                        .SelectMany((modifier) =>
                        {
                            return instrument.Strings
                                .Where((s, i) => modifier.IsFor(i))
                                .Select((s, i) => new ModifierOffset
                                {
                                    Offset = modifier.GetOffset(i),
                                    String = i
                                });

                        });
                    return new UserInstrumentModifier
                    {
                        Name = x.Name,
                        Offsets = offsets.ToList(),
                        Type = x.Type
                    };
                }).ToList(),
                Name = instrument.Name, 
                Type = instrument.Type,
                UserInstrumentId = Guid.NewGuid().ToString()
            };
        }
    }
}
