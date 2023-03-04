using NoteMapper.Core.Guitars;
using NoteMapper.Core.MusicTheory;
using NoteMapper.Services.Web.ViewModels.Instruments;

namespace NoteMapper.Services.Web.Tests.ViewModels.Instruments
{
    public static class InstrumentEditViewModelTests
    {
        private const string DefaultUserInstrumentId = "id";

        [Test]
        public static void AddModifier_Default_SetsDefaultType()
        {
            InstrumentEditViewModel viewModel = CreateViewModel();
            viewModel.AddModifier();

            string defaultType = viewModel.ModifierTypeOptions.First();

            string actual = viewModel.Modifiers.First().Type;
            Assert.That(actual, Is.EqualTo(defaultType));
        }

        [Test]
        public static void AddModifier_Instance_NoTypeSet_SetsDefaultType()
        {
            InstrumentEditViewModel viewModel = CreateViewModel();
            viewModel.AddModifier(new());

            string defaultType = viewModel.ModifierTypeOptions.First();

            string actual = viewModel.Modifiers.First().Type;
            Assert.That(actual, Is.EqualTo(defaultType));
        }

        [Test]
        public static void AddModifier_Instance_InvalidTypeSet_SetsDefaultType()
        {
            InstrumentEditViewModel viewModel = CreateViewModel();
            viewModel.AddModifier(new InstrumentModifierViewModel
            {
                Type = "INVALID"
            });

            string defaultType = viewModel.ModifierTypeOptions.First();

            string actual = viewModel.Modifiers.First().Type;

            Assert.That(actual, Is.EqualTo(defaultType));
        }

        [Test]
        public static void AddModifier_SetsModifierCountOnStrings()
        {
            InstrumentEditViewModel viewModel = CreateViewModel();

            for (int i = 0; i < 3; i++)
            {
                viewModel.AddString();
            }

            viewModel.AddModifier();

            for (int i = 0; i < 3; i++)
            {
                int actual = viewModel.Strings.ElementAt(i).ModifierOffsets.Count;
                Assert.That(actual, Is.EqualTo(1));
            }
        }

        [Test]
        public static void AddString_SetsModifierCount()
        {
            InstrumentEditViewModel viewModel = CreateViewModel();

            for (int i = 0; i < 3; i++)
            {
                viewModel.AddModifier();
            }

            viewModel.AddString();

            int actual = viewModel.Strings.First().ModifierOffsets.Count;

            Assert.That(actual, Is.EqualTo(3));
        }

        [TestCase(0, 1)]
        [TestCase(0, 2)]
        [TestCase(1, -1)]
        [TestCase(1, -2)]
        public static void MoveModifier_MovesModifierInCorrectDirection(int index, int direction)
        {
            InstrumentEditViewModel viewModel = CreateViewModel(modifiers: new[] { "A", "B" });

            viewModel.MoveModifier(viewModel.Modifiers.ElementAt(index), direction);

            CollectionAssert.AreEqual(new[]
            {
                "B", "A"
            }, viewModel.Modifiers.Select(x => x.Name));
        }

        [TestCase(0, -1)]
        [TestCase(0, -2)]
        [TestCase(1, 1)]
        [TestCase(1, 2)]
        public static void MoveModifier_DoesNotMoveModifierOutOfBounds(int index, int direction)
        {
            InstrumentEditViewModel viewModel = CreateViewModel(modifiers: new[] { "A", "B" });

            viewModel.MoveModifier(viewModel.Modifiers.ElementAt(index), direction);

            CollectionAssert.AreEqual(new[]
            {
                "A", "B"
            }, viewModel.Modifiers.Select(x => x.Name));
        }

        [Test]
        public static void MoveModifier_MovesStringOffsets()
        {
            InstrumentEditViewModel viewModel = CreateViewModel(modifiers: new[] { "A", "B" },
                strings: new[] { 9, 11 });

            // set up a 2 string instrument with 2 modifiers
            // modifier 0 has offset of +1, modifier 1 has offset of -1
            viewModel.Strings.ElementAt(0).ModifierOffsets.ElementAt(0).Offset = 1;
            viewModel.Strings.ElementAt(1).ModifierOffsets.ElementAt(0).Offset = 1;

            viewModel.Strings.ElementAt(0).ModifierOffsets.ElementAt(1).Offset = -1;
            viewModel.Strings.ElementAt(1).ModifierOffsets.ElementAt(1).Offset = -1;

            // switch the 2 modifiers
            viewModel.MoveModifier(viewModel.Modifiers.ElementAt(0), 1);
                
            // expect that modifier 0 has offset of -1, modifier 1 has offset of +1
            CollectionAssert.AreEqual(new[] { -1, -1 }, new[]
            {
                viewModel.Strings.ElementAt(0).ModifierOffsets.ElementAt(0).Offset = -1,
                viewModel.Strings.ElementAt(1).ModifierOffsets.ElementAt(0).Offset = -1
            });

            CollectionAssert.AreEqual(new[] { 1, 1 }, new[]
            {
                viewModel.Strings.ElementAt(0).ModifierOffsets.ElementAt(1).Offset = 1,
                viewModel.Strings.ElementAt(1).ModifierOffsets.ElementAt(1).Offset = 1
            });
        }

        [Test]
        public static void RemoveModifier_SetsModifierCountOnStrings()
        {
            InstrumentEditViewModel viewModel = CreateViewModel();

            for (int i = 0; i < 3; i++)
            {
                viewModel.AddString();
            }

            viewModel.AddModifier();

            viewModel.RemoveModifier(viewModel.Modifiers.First());

            for (int i = 0; i < 3; i++)
            {
                int actual = viewModel.Strings.ElementAt(i).ModifierOffsets.Count;
                Assert.That(actual, Is.EqualTo(0));
            }
        }

        private static InstrumentEditViewModel CreateViewModel(string id = DefaultUserInstrumentId,
            IEnumerable<string>? modifiers = null, IEnumerable<int>? strings = null)
        {
            InstrumentEditViewModel viewModel = new(id, GuitarType.PedalSteelGuitar, AccidentalType.Sharp);

            if (modifiers != null)
            {
                foreach (string modifier in modifiers)
                {
                    viewModel.AddModifier(new InstrumentModifierViewModel
                    {
                        Name = modifier
                    });
                }
            }

            if (strings != null)
            {
                foreach (int @string in strings)
                {
                    viewModel.AddString(new InstrumentStringViewModel(AccidentalType.Sharp)
                    {
                        NoteIndex = @string
                    });
                }
            }

            return viewModel;
        }
    }
}