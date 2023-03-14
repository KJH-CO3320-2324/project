using NoteMapper.Data.Core.Instruments;

namespace NoteMapper.Data.Mongo.Models.UserInstruments
{
    public class UserInstrumentsDto : MongoDto
    {        
        public List<UserInstrument> Instruments { get; set; } = new List<UserInstrument>();
    }
}
