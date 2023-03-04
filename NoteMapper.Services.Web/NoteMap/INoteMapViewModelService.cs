using NoteMapper.Core.Guitars;
using NoteMapper.Services.Users;
using NoteMapper.Services.Web.ViewModels.NoteMap;

namespace NoteMapper.Services.Web.NoteMap
{
    public interface INoteMapViewModelService
    {
        NoteMapCriteriaViewModel GetNoteMapCriteriaViewModel(UserPreferences preferences,
            NoteMapCriteriaOptionsViewModel options,
            string instrument, string note, IEnumerable<string> customNotes, string key, string type,
            string mode);

        Task<NoteMapCriteriaOptionsViewModel> GetNoteMapCriteriaOptionsViewModelAsync(Guid? userId);

        NoteMapViewModel? GetNoteMapPermutationsViewModel(GuitarBase? instrument, NoteMapOptionsViewModel options);
    }
}
