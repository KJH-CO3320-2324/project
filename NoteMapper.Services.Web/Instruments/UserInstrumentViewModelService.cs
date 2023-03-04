using NoteMapper.Core.Extensions;
using NoteMapper.Core.MusicTheory;
using NoteMapper.Data.Core.Instruments;
using NoteMapper.Data.Core.Users;
using NoteMapper.Services.Instruments;
using NoteMapper.Services.Users;
using NoteMapper.Services.Web.ViewModels.Instruments;

namespace NoteMapper.Services.Web.Instruments
{
    public class UserInstrumentViewModelService : IUserInstrumentViewModelService
    {
        private readonly IUserInstrumentService _userInstrumentService;
        private readonly IUserService _userService;

        public UserInstrumentViewModelService(IUserInstrumentService userInstrumentService,
            IUserService userService)
        {
            _userInstrumentService = userInstrumentService;
            _userService = userService;
        }

        public async Task<InstrumentEditViewModel?> GetInstrumentEditViewModelAsync(Guid userId, string userInstrumentId,
            AccidentalType accidental)
        {
            UserInstrument? userInstrument = await _userInstrumentService.FindUserInstrumentAsync(userId, userInstrumentId);
            if (userInstrument == null)
            {
                return null;
            }

            return await MapUserInstrumentToEditViewModelAsync(userId, userInstrument);
        }

        public async Task<InstrumentEditViewModel> MapUserInstrumentToEditViewModelAsync(Guid? userId, UserInstrument userInstrument)
        {
            UserPreferences preferences = await _userService.GetPreferences(userId);

            InstrumentEditViewModel viewModel = new(userInstrument.UserInstrumentId,
                userInstrument.Type, preferences.Accidental)
            {
                Name = userInstrument.Name
            };

            AddStringViewModels(userInstrument, viewModel, preferences.Accidental);
            AddModifierViewModels(userInstrument, viewModel);

            return viewModel;
        }

        public void MapEditViewModelToUserInstrument(InstrumentEditViewModel viewModel, UserInstrument userInstrument)
        {
            userInstrument.Name = viewModel.Name;

            // strings
            userInstrument.Strings.Clear();

            foreach (InstrumentStringViewModel @string in viewModel.Strings)
            {
                userInstrument.Strings.Add(new UserInstrumentString
                {
                    NoteIndex = @string.NoteIndex,
                    OctaveIndex = @string.Octave
                });
            }

            // modifiers
            userInstrument.Modifiers.Clear();

            IEnumerable<UserInstrumentModifier> modifiers = ToUserInstrumentModifiers(viewModel);
            userInstrument.Modifiers.AddRange(modifiers);
        }

        private static void SetIncompatibleViewModelModifiers(UserInstrument userInstrument,
            InstrumentEditViewModel instrumentViewModel)
        {
            for (int index = 0; index < userInstrument.Modifiers.Count; index++)
            {
                UserInstrumentModifier modifier = userInstrument.Modifiers[index];
                if (modifier.MutuallyExclusive == null)
                {
                    continue;
                }

                foreach (string incompatibleModifier in modifier.MutuallyExclusive)
                {
                    UserInstrumentModifier? other = userInstrument.Modifiers
                        .FirstOrDefault(x => string.Equals(x.Name, incompatibleModifier, StringComparison.InvariantCultureIgnoreCase));
                    if (other == null)
                    {
                        continue;
                    }

                    int otherIndex = userInstrument.Modifiers.IndexOf(other);

                    IReadOnlyCollection<InstrumentModifierViewModel> modifierViewModels = instrumentViewModel.Modifiers;

                    instrumentViewModel.SetIncompatibleModifiers(index, otherIndex);
                }
            }
        }

        private static void SetViewModelOffsets(UserInstrument userInstrument, InstrumentEditViewModel viewModel)
        {
            for (int i = 0; i < viewModel.Strings.Count; i++)
            {
                InstrumentStringViewModel @string = viewModel.Strings.ElementAt(i);
                @string.SetModifierOffsetCount(userInstrument.Modifiers.Count);

                for (int modifierIndex = 0; modifierIndex < userInstrument.Modifiers.Count; modifierIndex++)
                {
                    UserInstrumentModifier modifier = userInstrument.Modifiers.ElementAt(modifierIndex);
                    ModifierOffset? offset = modifier.Offsets.FirstOrDefault(x => x.String == i);
                    @string.ModifierOffsets.ElementAt(modifierIndex).Offset = offset != null
                        ? offset.Offset
                        : 0;
                }
            }
        }

        private static void AddModifierViewModels(UserInstrument userInstrument, InstrumentEditViewModel instrumentViewModel)
        {
            for (int i = 0; i < userInstrument.Modifiers.Count; i++)
            {
                UserInstrumentModifier modifier = userInstrument.Modifiers.ElementAt(i);
                InstrumentModifierViewModel viewModel = new()
                {
                    Name = modifier.Name,
                    Type = modifier.Type
                };

                instrumentViewModel.AddModifier(viewModel);
            }

            SetViewModelOffsets(userInstrument, instrumentViewModel);
            SetIncompatibleViewModelModifiers(userInstrument, instrumentViewModel);
        }

        private static void AddStringViewModels(UserInstrument userInstrument, InstrumentEditViewModel instrumentViewModel,
            AccidentalType accidental)
        {
            for (int i = 0; i < userInstrument.Strings.Count; i++)
            {
                UserInstrumentString @string = userInstrument.Strings.ElementAt(i);

                InstrumentStringViewModel viewModel = new(accidental)
                {
                    NoteIndex = @string.NoteIndex,
                    Octave = @string.OctaveIndex
                };

                instrumentViewModel.AddString(viewModel);
            }
        }

        private static IEnumerable<UserInstrumentModifier> ToUserInstrumentModifiers(InstrumentEditViewModel viewModel)
        {
            for (int modifierIndex = 0; modifierIndex < viewModel.Modifiers.Count; modifierIndex++)
            {
                InstrumentModifierViewModel modifierViewModel = viewModel.Modifiers.ElementAt(modifierIndex);

                List<ModifierOffset> offsets = new();

                for (int stringIndex = 0; stringIndex < viewModel.Strings.Count; stringIndex++)
                {
                    InstrumentStringViewModel stringViewModel = viewModel.Strings.ElementAt(stringIndex);
                    StringOffsetViewModel offsetViewModel = stringViewModel.ModifierOffsets.ElementAt(modifierIndex);
                    if (offsetViewModel.Offset == 0)
                    {
                        continue;
                    }

                    offsets.Add(new ModifierOffset
                    {
                        Offset = offsetViewModel.Offset,
                        String = stringIndex
                    });
                }

                List<string> mutuallyExclusive = new();

                foreach (InstrumentModifierViewModel other in modifierViewModel.IncompatibleModifiers)
                {
                    int index = viewModel.Modifiers.IndexOf(modifierViewModel);
                    int otherIndex = viewModel.Modifiers.IndexOf(other);

                    if (otherIndex > index)
                    {
                        mutuallyExclusive.Add(other.Name);
                    }
                }

                yield return new UserInstrumentModifier
                {
                    MutuallyExclusive = mutuallyExclusive.Count > 0
                        ? mutuallyExclusive
                        : null,
                    Name = modifierViewModel.Name,
                    Offsets = offsets,
                    Type = modifierViewModel.Type
                };
            }
        }
    }
}
