using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameServer.Models
{
    //Stored in "saves" 
    //one document per save slot per user.
    public class SaveGame
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id{get; set;}

        //which user owns save
        [BsonElement("userId")]
        public string userId {get; set;} = string.empty;

        // slot1, 2, 3 & autoSave
        [BsonElement("slot")]
        public string Slot {get; set;} = string.empty;
    }
}