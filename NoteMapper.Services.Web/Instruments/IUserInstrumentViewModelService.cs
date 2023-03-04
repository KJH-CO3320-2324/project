using NoteMapper.Core.MusicTheory;
using NoteMapper.Data.Core.Instruments;
using NoteMapper.Services.Web.ViewModels.Instruments;

namespace NoteMapper.Services.Web.Instruments
{
    public interface IUserInstrumentViewModelService
    {
        Task<InstrumentEditViewModel?> GetInstrumentEditViewModelAsync(Guid userId, string userInstrumentId,
            AccidentalType accidental);

        Task<InstrumentEditViewModel> MapUserInstrumentToEditViewModelAsync(Guid? userId, UserInstrument userInstrument);

        void MapEditViewModelToUserInstrument(InstrumentEditViewModel viewModel, UserInstrument userInstrument);
    }
}
