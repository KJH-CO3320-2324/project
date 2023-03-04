using NoteMapper.Core.Guitars;
using NoteMapper.Data.Core.Instruments;

namespace NoteMapper.Services
{
    public interface IInstrumentFactory
    {
        GuitarBase FromUserInstrument(UserInstrument userInstrument);

        UserInstrument ToUserInstrument(GuitarBase instrument);
    }
}
