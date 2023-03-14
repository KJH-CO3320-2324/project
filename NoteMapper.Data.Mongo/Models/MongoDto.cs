using Newtonsoft.Json;

namespace NoteMapper.Data.Mongo.Models
{    
    public abstract class MongoDto
    {
        // Mongo
        [JsonProperty(MongoUtils.IdColumn)]
        public string Id { get; set; } = "";
    }
}
