using NoteMapper.Core.Extensions;
using NoteMapper.Core.Guitars;
using NoteMapper.Core.MusicTheory;
using NoteMapper.Core.NoteMap;
using NoteMapper.Data.Core.Instruments;
using NoteMapper.Services.Users;
using NoteMapper.Services.Web.ViewModels.NoteMap;

namespace NoteMapper.Services.Web.NoteMap
{
    public class NoteMapViewModelService : INoteMapViewModelService
    {
        private readonly IInstrumentFactory _instrumentFactory;
        private readonly IMusicTheoryService _musicTheoryService;
        private readonly IUserInstrumentRepository _userInstrumentRepository;

        public NoteMapViewModelService(IInstrumentFactory instrumentFactory, IMusicTheoryService musicTheoryService,
            IUserInstrumentRepository userInstrumentRepository)
        {
            _instrumentFactory = instrumentFactory;
            _musicTheoryService = musicTheoryService;
            _userInstrumentRepository = userInstrumentRepository;
        }

        public NoteMapCriteriaViewModel GetNoteMapCriteriaViewModel(UserPreferences preferences,
            NoteMapCriteriaOptionsViewModel options,
            string instrument, string note, IEnumerable<string> customNotes, string key, string type, string mode)
        {
            if (string.IsNullOrEmpty(instrument))
            {
                instrument = options?.DefaultInstruments.FirstOrDefault()?.Id ?? "";
            }

            int.TryParse(note, out int noteIndex);

            ScaleType scaleType = Scale.ParseType(key);
            Scale? keyScale = Scale.TryParse(noteIndex, scaleType);

            if (!Enum.TryParse(type, true, out NoteCollectionType parsedType))
            {
                parsedType = NoteMapCriteriaViewModel.DefaultType;
            }

            if (!Enum.TryParse(mode, true, out NoteMapMode parsedMode))
            {
                parsedMode = NoteMapCriteriaViewModel.DefaultMode;
            }

            HashSet<int> parsedCustomNotes = new();
            foreach (string s in customNotes)
            {
                if (int.TryParse(s, out int parsedCustomNote))
                {
                    parsedCustomNotes.Add(parsedCustomNote);
                }
            }

            return new NoteMapCriteriaViewModel
            {
                CustomNotes = parsedCustomNotes,
                InstrumentId = instrument,
                Mode = parsedMode,
                NoteIndex = noteIndex,
                ScaleType = keyScale?.Type ?? ScaleType.Major,
                Type = parsedType
            };
        }

        public async Task<NoteMapCriteriaOptionsViewModel> GetNoteMapCriteriaOptionsViewModelAsync(Guid? userId)
        {
            IReadOnlyCollection<UserInstrument> defaultInstruments = await _userInstrumentRepository.GetDefaultInstrumentsAsync();
            IReadOnlyCollection<UserInstrument> userInstruments = userId != null
                ? await _userInstrumentRepository.GetUserInstrumentsAsync(userId.Value)
                : Array.Empty<UserInstrument>();

            IReadOnlyCollection<(int, int?)> noteIndexes = _musicTheoryService.GetNaturalNoteIndexesWithSharps();
            IReadOnlyCollection<ScaleType> scaleTypes = _musicTheoryService.GetScaleTypes();
            return new NoteMapCriteriaOptionsViewModel(
                defaultInstruments.Select(_instrumentFactory.FromUserInstrument),
                userInstruments.Select(_instrumentFactory.FromUserInstrument),
                noteIndexes,
                scaleTypes);
        }

        public NoteMapViewModel? GetNoteMapPermutationsViewModel(GuitarBase? instrument,
            NoteMapOptionsViewModel options)
        {
            if (instrument == null)
            {
                return default;
            }

            INoteCollection notes = Note.GetNotes(options.NoteIndex, options.ScaleType,
                options.Type, options.CustomNotes);

            int frets = instrument.Strings.Max(x => x.Frets);

            NoteMapViewModel viewModel = new();

            for (int fret = 0; fret <= frets; fret++)
            {
                NoteMapFretViewModel fretViewModel = new(fret);

                int threshold = options.Type == NoteCollectionType.Chord || options.Type == NoteCollectionType.Custom
                    ? notes.Count
                    : 1;
                StringPermutationOptions permutationOptions = new(notes, fret, threshold);

                List<IReadOnlyCollection<GuitarStringNote?>> permutations = new();

                if (options.Mode == NoteMapMode.Manual)
                {
                    IReadOnlyCollection<GuitarStringNote?> singlePermutation = instrument
                        .GetNotes(options.Modifiers.ToArray(), permutationOptions)
                        .ToArray();
                    if (singlePermutation.Count > 0)
                    {
                        permutations.Add(singlePermutation);
                    }
                }
                else if (options.Mode == NoteMapMode.Combinations)
                {
                    permutations.AddRange(instrument.GetPermutations(permutationOptions));
                }

                foreach (IReadOnlyCollection<GuitarStringNote?> permutation in permutations)
                {
                    NoteMapNotesViewModel permutationViewModel = new(permutation, notes.Key);
                    fretViewModel.AddPermutation(permutationViewModel);
                }

                viewModel.AddFret(fretViewModel);
            }

            return viewModel;
        }
    }
}
