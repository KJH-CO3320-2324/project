using System.ComponentModel.DataAnnotations;
using NoteMapper.Core.Extensions;
using NoteMapper.Core.Guitars;
using NoteMapper.Core.MusicTheory;

namespace NoteMapper.Services.Web.ViewModels.Instruments
{
    public class InstrumentEditViewModel
    {
        private readonly List<InstrumentModifierViewModel> _modifiers = new();
        private readonly List<InstrumentStringViewModel> _strings = new();

        public InstrumentEditViewModel(string id, GuitarType type, AccidentalType accidental)
        {
            Accidental = accidental;
            Id = id;
            Type = type;

            IReadOnlyCollection<string> notes = Note.GetNotes(accidental).ToArray();

            ModifierTypeOptions = type.ModifierTypes().ToArray();
            NoteOptions = notes
                .Select((x, i) => new KeyValuePair<int, string>(i, x))
                .ToArray();
            OctaveOptions = Note.GetOctaves().ToArray();
        }

        public AccidentalType Accidental { get; }

        public string Id { get; }

        public IReadOnlyCollection<InstrumentModifierViewModel> Modifiers => _modifiers;

        [Required]
        public string Name { get; set; } = "";

        public IReadOnlyCollection<string> ModifierTypeOptions { get; }

        public IReadOnlyCollection<KeyValuePair<int, string>> NoteOptions { get; }

        public IReadOnlyCollection<int> OctaveOptions { get; }

        public IReadOnlyCollection<InstrumentStringViewModel> Strings => _strings;

        public GuitarType Type { get; }

        public void AddModifier() => AddModifier(new());

        public void AddModifier(InstrumentModifierViewModel modifier)
        {
            if (string.IsNullOrEmpty(modifier.Type) || !ModifierTypeOptions.Contains(modifier.Type))
            {
                modifier.Type = ModifierTypeOptions.FirstOrDefault() ?? "";
            }

            _modifiers.Add(modifier);

            foreach (InstrumentStringViewModel s in _strings)
            {
                s.SetModifierOffsetCount(_modifiers.Count);
            }
        }

        public void AddString()
        {
            InstrumentStringViewModel? previous = _strings.LastOrDefault();

            InstrumentStringViewModel @string = new(Accidental)
            {
                NoteIndex = previous?.NoteIndex ?? 0,
                Octave = previous?.Octave ?? Note.GetOctaves().First()
            };

            AddString(@string);
        }

        public void AddString(InstrumentStringViewModel @string)
        {
            @string.SetModifierOffsetCount(_modifiers.Count);

            _strings.Add(@string);
        }

        public void MoveModifier(InstrumentModifierViewModel modifier, int direction)
        {
            int oldIndex = _modifiers.IndexOf(modifier);
            _modifiers.MoveOne(modifier, direction);
            int newIndex = _modifiers.IndexOf(modifier);

            if (oldIndex == newIndex)
            {
                return;
            }

            foreach (InstrumentStringViewModel @string in Strings)
            {
                @string.SwitchOffsets(oldIndex, newIndex);
            }
        }

        public void RemoveModifier(InstrumentModifierViewModel modifier)
        {
            int index = _modifiers.IndexOf(modifier);

            _modifiers.Remove(modifier);

            foreach (InstrumentStringViewModel @string in Strings)
            {
                @string.RemoveOffsets(index);
            }

            foreach (InstrumentModifierViewModel m in Modifiers)
            {
                m.IncompatibleModifiers.Remove(modifier);
            }
        }

        public void RemoveString(InstrumentStringViewModel @string)
        {
            _strings.Remove(@string);
        }

        public void SetIncompatibleModifiers(int index, int otherIndex)
        {
            if (index == otherIndex)
            {
                return;
            }

            _modifiers[index].IncompatibleModifiers.Add(_modifiers[otherIndex]);
            _modifiers[otherIndex].IncompatibleModifiers.Add(_modifiers[index]);
        }

        public void ToggleIncompatibleModifier(InstrumentModifierViewModel modifier, 
            InstrumentModifierViewModel other)
        {
            if (modifier.IncompatibleModifiers.Contains(other))
            {
                modifier.IncompatibleModifiers.Remove(other);
                other.IncompatibleModifiers.Remove(modifier);
            }
            else
            {
                modifier.IncompatibleModifiers.Add(other);
                other.IncompatibleModifiers.Add(modifier);
            }
        }
    }
}
